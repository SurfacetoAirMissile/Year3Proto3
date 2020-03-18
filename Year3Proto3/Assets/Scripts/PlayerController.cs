using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Tooltip("How quickly the player moves.")]
    private float movementSpeed = 3;

    [SerializeField] [Tooltip("How quickly the player leans. (x would mean a full lean in 1/x seconds)")]
    private float leanSpeed = 3;

    [SerializeField]
    private AnimationCurve leanCurve;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Mathf.Clamp(Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
        cameraPitch += Mathf.Clamp(Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f);
        //Debug.Log(cameraPitch);
        transform.parent.Rotate(new Vector3(0f, mouseX, 0f));
        UpdateMove();
        UpdateCamera();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * 200f);
        }
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
        Camera.main.transform.LookAt(transform.parent.position + Camera.main.transform.localPosition + cameraLookTarget);
        //float upDifference = frontOffset.z + backOffset.z;
        //if (frontOffset != Vector3.zero || backOffset != Vector3.zero)
        //{
            //Camera.main.transform.Rotate(new Vector3(upDifference, 0f, 0f));
        //}
        
    }

    void UpdateMove()
    {
        float movementAmount = movementSpeed * Time.deltaTime * 1000f;
        Vector3 forceTotal = Vector3.zero;

        if (Input.GetKey(forwardKey)) { forceTotal += transform.parent.forward; }
        if (Input.GetKey(backKey)) { forceTotal -= transform.parent.forward; }

        if (Input.GetKey(rightKey)) { forceTotal += transform.parent.right; }
        if (Input.GetKey(leftKey)) { forceTotal -= transform.parent.right; }

        if (forceTotal != Vector3.zero)
        {
            playerRB.AddForce(forceTotal.normalized * movementAmount);
        }
    }
}
