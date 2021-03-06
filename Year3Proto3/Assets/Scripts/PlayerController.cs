﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Tooltip("How quickly the player moves.")]
    private float movementSpeed = 4;

    [SerializeField] [Tooltip("How quickly the player leans. (x would mean a full lean in 1/x seconds)")]
    private float leanSpeed = 3;

    [SerializeField]
    private AnimationCurve leanCurve = new AnimationCurve();

    [SerializeField]
    private float mouseSensitivity = 90f;

    private Transform cameraRestPosition, rightLeanTarget, leftLeanTarget, frontLeanTarget, backLeanTarget;
    private float rightLeanAmount, leftLeanAmount, frontLeanAmount, backLeanAmount;
    private Rigidbody playerRB = null;
    private KeyCode forwardKey = KeyCode.W;
    private KeyCode backKey = KeyCode.S;
    private KeyCode leftKey = KeyCode.A;
    private KeyCode rightKey = KeyCode.D;
    private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    private float cameraPitch = 0f;
    private Quaternion physicsObjectRotation;

    public Enemy hackableEnemy = null;
    public Door hackableDoor = null;

    public Vector3 puzzleDestination = Vector3.zero;
    public Vector3 puzzleLookDestination = Vector3.zero;
    public bool isHacking = false;
    public Puzzle currentPuzzle;
    public bool lerpingToPuzzle = false;
    public float lerpTime = 0.2f;
    public float currentTime = 0.0f;
    public Vector3 startPosition = Vector3.zero;
    public Vector3 startLookPosition = Vector3.zero;

    public GameObject physicsObject;
    public ConsoleBehaviour console;
    public Vector3[] lastFramePositions;
    private Animator playerAnim = null;
    private bool lookingAtEnemy = false;
    private int secondPuzzle;

    public Vector3 GetPhysicsVelocity()
    {
        float timeScale = 1f / Time.fixedDeltaTime;
        // Each frame we step back, the velocity has less relevance on the velocity calculated.
        Vector3 sum = (physicsObject.transform.position - lastFramePositions[0]) * 5f;
        for (int i = 0; i < 4; i++)
        {
            Vector3 difference = (lastFramePositions[i] - lastFramePositions[i + 1]) * (4 - i);
            sum += difference;
        }
        // 15 = 5 + 4 + 3 + 2 + 1
        return (sum / 15f) * timeScale;
    }

    public void UpdatePhysicsFrames(Vector3 _lastFramePosition)
    {
        lastFramePositions[4] = lastFramePositions[3];
        lastFramePositions[3] = lastFramePositions[2];
        lastFramePositions[2] = lastFramePositions[1];
        lastFramePositions[1] = lastFramePositions[0];
        lastFramePositions[0] = _lastFramePosition;
    }

    // I'm using Awake for initialization of variables.
    // Awake is called on creation of the object (before Start)
    private void Awake()
    {
        // initialize Camera Rest Position
        cameraRestPosition = transform.Find("CameraRestPosition");
        // initialize Lean Offsets
        for (int i = 1; i < transform.childCount; i++)
        {
            Transform childI = transform.GetChild(i);
            if (childI.name.Contains("RightLeanTarget")) { rightLeanTarget = childI; }
            if (childI.name.Contains("LeftLeanTarget")) { leftLeanTarget = childI; }
            if (childI.name.Contains("FrontLeanTarget")) { frontLeanTarget = childI; }
            if (childI.name.Contains("BackLeanTarget")) { backLeanTarget = childI; }
        }
        // initialize Lean Amounts
        rightLeanAmount = 0f; leftLeanAmount = 0f; frontLeanAmount = 0f; backLeanAmount = 0f;
        // initialize Player Rigidbody
        playerRB = transform.parent.GetComponent<Rigidbody>();
        lastFramePositions = new Vector3[5] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        secondPuzzle = 2;
        playerAnim = transform.parent.Find("Main Camera/Arms").GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(0.50f);
        currentPuzzle.GetComponent<HologramFX>().showHologram = false;
        currentPuzzle.GetComponentInParent<DestroyMe>().Prime();
        currentPuzzle = null;
    }

    IEnumerator StartSecond()
    {
        yield return new WaitForSeconds(0.51f);
        StartHack(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (physicsObject)
        {
            UpdatePhysicsFrames(physicsObject.transform.position);
            Vector3 targetPos = Camera.main.transform.position + (Camera.main.transform.forward * 2f);

            physicsObject.transform.position = targetPos;
            physicsObject.transform.rotation = Quaternion.Lerp(physicsObject.transform.rotation, transform.rotation, Time.deltaTime * 60.0f);

            Rigidbody rigid = physicsObject.GetComponent<Rigidbody>();
            if(rigid.useGravity) physicsObjectRotation = physicsObject.transform.rotation;
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            rigid.useGravity = false;
        }
        if (GameManager.Instance.playerControl)
        {
            UpdateMove();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit raycastHit, 2f, 1 << LayerMask.NameToLayer("Console")))
        {
            ConsoleBehaviour consoleScript = raycastHit.transform.parent.GetComponent<ConsoleBehaviour>();
            if (consoleScript.active)
            {
                hackableDoor = consoleScript.door;
                FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Hacking);
            }
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 2f))
        {
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("PhysicsObject"))
            {
                FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Pickup);
            }
            else
            {
                hackableDoor = null;
                if (raycastHit.transform.CompareTag("Enemy"))
                {
                    if (raycastHit.transform.GetComponentInParent<Enemy>().isActive())
                    {
                        lookingAtEnemy = true;
                        FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Hacking);
                    }
                    else
                    {
                        lookingAtEnemy = false;
                        FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.None);
                    }
                }
                else
                {
                    FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.None);
                }
            }
        }
        else
        {
            lookingAtEnemy = false;
            hackableDoor = null;
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.None);
        }
        if (GameManager.Instance.playerControl)
        {
            if (physicsObject)
            {
                FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Holding);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 2f, 1 << LayerMask.NameToLayer("Console")))
                {
                    ConsoleBehaviour consoleScript = raycastHit.transform.parent.GetComponent<ConsoleBehaviour>();
                    if (consoleScript.active)
                    {
                        console = consoleScript;
                        hackableDoor = console.door;
                        hackableEnemy = null;
                        puzzleDestination = transform.position;
                        puzzleLookDestination = raycastHit.transform.parent.GetChild(2).position;
                    }
                }
                if (hackableEnemy && lookingAtEnemy || hackableDoor)
                {
                    AttemptHack();
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Raycast to find a physics object and try to pick it up
                //physicsObject
                if (!physicsObject)
                {
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 2f, 1 << LayerMask.NameToLayer("PhysicsObject")))
                    {
                        if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("PhysicsObject"))
                        {
                            physicsObject = raycastHit.transform.gameObject;
                            physicsObject.GetComponent<Rigidbody>().useGravity = false;
                        }
                    }
                }
                else
                {
                    Rigidbody rigid = physicsObject.GetComponent<Rigidbody>();
                    rigid.useGravity = true;
                    rigid.velocity = GetPhysicsVelocity();
                    physicsObject = null;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (physicsObject)
                {
                    Rigidbody rigid = physicsObject.GetComponent<Rigidbody>();
                    rigid.useGravity = true;
                    rigid.velocity = GetPhysicsVelocity();
                    rigid.AddForce(Camera.main.transform.forward * 100f);
                    physicsObject = null;
                }
            }
            float mouseX = Mathf.Clamp(Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
            cameraPitch += Mathf.Clamp(Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
            cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f);
            //Debug.Log(cameraPitch);
            transform.parent.Rotate(new Vector3(0f, mouseX, 0f));
            UpdateCamera();
        }
        else
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Puzzle);
            if (lerpingToPuzzle)
            {
                currentTime += Time.deltaTime;
                if (currentTime < lerpTime)
                {
                    LerpToPuzzle(currentTime / lerpTime);
                }
                else
                {
                    LerpToPuzzle(1f);
                    StartHack(hackableDoor);
                }
            }
            if (isHacking)
            {
                if (currentPuzzle)
                {
                    if (currentPuzzle.Validate())
                    {
                        StartCoroutine(WaitForAnimation());
                        if (hackableEnemy)
                        {
                            isHacking = false;
                            if (secondPuzzle != 2)
                            {
                                StartCoroutine(StartSecond());
                            }
                            else
                            {
                                hackableEnemy.SwitchState(Enemy.AIState.deactivated);
                                hackableEnemy.isBeingHacked = false;
                                hackableEnemy.transform.GetChild(2).GetComponent<AudioSource>().DOFade(0f, 0.25f);
                                hackableEnemy = null;
                                GameManager.Instance.playerControl = true;
                            }
                        }
                        if (hackableDoor)
                        {
                            console.active = false;
                            secondPuzzle = 2;
                            console = null;
                            hackableDoor.ToggleDoorOpen();
                            hackableDoor = null;
                            isHacking = false;
                            GameManager.Instance.playerControl = true;
                        }
                    }
                }
            }
                
        }
        playerAnim.SetBool("Hacking", isHacking || secondPuzzle != 2);
        playerAnim.SetBool("Ready", hackableDoor || (hackableEnemy && lookingAtEnemy));
    }

    void UpdateCamera()
    {
        // --- Calculating Lean ---
        // Front
        if (Input.GetKey(forwardKey)) { frontLeanAmount += Time.deltaTime * leanSpeed; }
        else { frontLeanAmount -= Time.deltaTime * leanSpeed; }
        frontLeanAmount = Mathf.Clamp(frontLeanAmount, 0f, 1f);
        Vector3 frontLean = Vector3.Lerp(cameraRestPosition.position, frontLeanTarget.position, leanCurve.Evaluate(frontLeanAmount));
        Vector3 frontOffset = frontLean - cameraRestPosition.position;

        // Back
        if (Input.GetKey(backKey)) { backLeanAmount += Time.deltaTime * leanSpeed; }
        else { backLeanAmount -= Time.deltaTime * leanSpeed; }
        backLeanAmount = Mathf.Clamp(backLeanAmount, 0f, 1f);
        Vector3 backLean = Vector3.Lerp(cameraRestPosition.position, backLeanTarget.position, leanCurve.Evaluate(backLeanAmount));
        Vector3 backOffset = backLean - cameraRestPosition.position;

        // Right
        if (Input.GetKey(rightKey)) { rightLeanAmount += Time.deltaTime * leanSpeed; }
        else { rightLeanAmount -= Time.deltaTime * leanSpeed; }
        rightLeanAmount = Mathf.Clamp(rightLeanAmount, 0f, 1f);
        Vector3 rightLean = Vector3.Lerp(cameraRestPosition.position, rightLeanTarget.position, leanCurve.Evaluate(rightLeanAmount));
        Vector3 rightOffset = rightLean - cameraRestPosition.position;

        // Left
        if (Input.GetKey(leftKey)) { leftLeanAmount += Time.deltaTime * leanSpeed; }
        else { leftLeanAmount -= Time.deltaTime * leanSpeed; }
        leftLeanAmount = Mathf.Clamp(leftLeanAmount, 0f, 1f);
        Vector3 leftLean = Vector3.Lerp(cameraRestPosition.position, leftLeanTarget.position, leanCurve.Evaluate(leftLeanAmount));
        Vector3 leftOffset = leftLean - cameraRestPosition.position;

        // --- Calculating Camera Position & Orientation ---

        //Camera.main.transform.position = cameraRestPosition.position + frontOffset + backOffset + rightOffset + leftOffset;
        //Camera.main.transform.position = cameraRestPosition.position + rightOffset + leftOffset;

        //Vector3 cameraUp = (Camera.main.transform.position - transform.parent.position).normalized;

        Vector3 cameraLookTarget = transform.parent.forward;

        cameraLookTarget = Vector3.RotateTowards(cameraLookTarget, cameraPitch > 0f ? Vector3.up : Vector3.down, Mathf.Abs(cameraPitch) * Mathf.Deg2Rad, 0f);

        //Camera.main.transform.LookAt(transform.parent.position + Camera.main.transform.localPosition + cameraLookTarget, cameraUp);
        //Camera.main.transform.LookAt(transform.parent.position + Camera.main.transform.localPosition + cameraLookTarget);
        Camera.main.transform.LookAt(Camera.main.transform.position + cameraLookTarget);
        //float upDifference = frontOffset.z + backOffset.z;
        //if (frontOffset != Vector3.zero || backOffset != Vector3.zero)
        //{
            //Camera.main.transform.Rotate(new Vector3(upDifference, 0f, 0f));
        //}
        
    }

    void UpdateMove()
    {
        float movementAmount = movementSpeed * 5f;
        Vector3 forceTotal = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift)) movementAmount *= 1.3f;

        if (Input.GetKey(forwardKey)) { forceTotal += transform.parent.forward; }
        if (Input.GetKey(backKey)) { forceTotal -= transform.parent.forward; }

        if (Input.GetKey(rightKey)) { forceTotal += transform.parent.right; }
        if (Input.GetKey(leftKey)) { forceTotal -= transform.parent.right; }

        if (forceTotal != Vector3.zero)
        {
            playerRB.AddForce(forceTotal.normalized * movementAmount);
        }
    }

    void AttemptHack()
    {
        if (hackableEnemy && !isHacking)
        {
            currentTime = 0f;
            startPosition = transform.parent.position;
            startLookPosition = transform.parent.GetChild(1).forward;
            lerpingToPuzzle = true;
            GameManager.Instance.playerControl = false;
            hackableEnemy.isBeingHacked = true;
        }
        else if (hackableDoor && !isHacking)
        {
            currentTime = 0f;
            startPosition = transform.parent.position;
            startLookPosition = transform.parent.GetChild(1).forward;
            lerpingToPuzzle = true;
            GameManager.Instance.playerControl = false;
        }
    }

    void StartHack(bool _door)
    {
        if (_door)
        {
            currentPuzzle = Instantiate(GameManager.Instance.CreatePuzzle(GameManager.PuzzleID.ring)).GetComponentInChildren<Puzzle>();
        }
        else
        {
            if (secondPuzzle != 2)
            {
                currentPuzzle = Instantiate(GameManager.Instance.CreatePuzzle((GameManager.PuzzleID)secondPuzzle)).GetComponentInChildren<Puzzle>();
                secondPuzzle = 2;
            }
            else
            {
                int puzzle = Random.Range(0, 2);
                secondPuzzle = 1 - puzzle;
                currentPuzzle = Instantiate(GameManager.Instance.CreatePuzzle((GameManager.PuzzleID)puzzle)).GetComponentInChildren<Puzzle>();
            }
        }
        currentPuzzle.transform.parent.position = Camera.main.transform.position + (Camera.main.transform.forward * 0.4f);
        currentPuzzle.transform.parent.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f);
        currentPuzzle.GetComponent<HologramFX>().showHologram = true;
        currentPuzzle.transform.parent.LookAt(transform);
        isHacking = true;
        lerpingToPuzzle = false;
    }

    void LerpToPuzzle(float _amount)
    {
        playerRB.velocity = Vector3.zero;
        Vector3 targetPosition = puzzleDestination;
        targetPosition.y = transform.parent.position.y;
        transform.parent.GetChild(0).LookAt(Vector3.Lerp(startLookPosition, puzzleLookDestination, _amount));
        transform.parent.position = Vector3.Lerp(startPosition, targetPosition, _amount);
    }
}
