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

    private Vector3 cameraRestPosition, rightLeanTarget, leftLeanTarget, frontLeanTarget, backLeanTarget;
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
        cameraRestPosition = transform.Find("CameraRestPosition").localPosition + transform.localPosition;
        // initialize Lean Offsets
        for (int i = 1; i < transform.childCount; i++)
        {
            Transform childI = transform.GetChild(i);
            if (childI.name.Contains("RightLeanTarget")) { rightLeanTarget = childI.localPosition + transform.localPosition; }
            if (childI.name.Contains("LeftLeanTarget")) { leftLeanTarget = childI.localPosition + transform.localPosition; }
            if (childI.name.Contains("FrontLeanTarget")) { frontLeanTarget = childI.localPosition + transform.localPosition; }
            if (childI.name.Contains("BackLeanTarget")) { backLeanTarget = childI.localPosition + transform.localPosition; }
        }
        // initialize Lean Amounts
        rightLeanAmount = 0f; leftLeanAmount = 0f; frontLeanAmount = 0f; backLeanAmount = 0f;
        // initialize Player Rigidbody
        playerRB = transform.parent.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Mathf.Clamp(Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
        cameraPitch += Mathf.Clamp(Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f);
        Debug.Log(cameraPitch);
        transform.parent.Rotate(new Vector3(0f, mouseX, 0f));
        UpdateMove();
        UpdateCamera();
    }

    void UpdateCamera()
    {
        // --- Calculating Lean ---
        // Front
        if (Input.GetKey(forwardKey)) { frontLeanAmount += Time.deltaTime * leanSpeed; }
        else { frontLeanAmount -= Time.deltaTime * leanSpeed; }
        frontLeanAmount = Mathf.Clamp(frontLeanAmount, 0f, 1f);
        Vector3 frontLean = Vector3.Lerp(cameraRestPosition, frontLeanTarget, leanCurve.Evaluate(frontLeanAmount));
        Vector3 frontOffset = frontLean - cameraRestPosition;

        // Back
        if (Input.GetKey(backKey)) { backLeanAmount += Time.deltaTime * leanSpeed; }
        else { backLeanAmount -= Time.deltaTime * leanSpeed; }
        backLeanAmount = Mathf.Clamp(backLeanAmount, 0f, 1f);
        Vector3 backLean = Vector3.Lerp(cameraRestPosition, backLeanTarget, leanCurve.Evaluate(backLeanAmount));
        Vector3 backOffset = backLean - cameraRestPosition;

        // Right
        if (Input.GetKey(rightKey)) { rightLeanAmount += Time.deltaTime * leanSpeed; }
        else { rightLeanAmount -= Time.deltaTime * leanSpeed; }
        rightLeanAmount = Mathf.Clamp(rightLeanAmount, 0f, 1f);
        Vector3 rightLean = Vector3.Lerp(cameraRestPosition, rightLeanTarget, leanCurve.Evaluate(rightLeanAmount));
        Vector3 rightOffset = rightLean - cameraRestPosition;

        // Left
        if (Input.GetKey(leftKey)) { leftLeanAmount += Time.deltaTime * leanSpeed; }
        else { leftLeanAmount -= Time.deltaTime * leanSpeed; }
        leftLeanAmount = Mathf.Clamp(leftLeanAmount, 0f, 1f);
        Vector3 leftLean = Vector3.Lerp(cameraRestPosition, leftLeanTarget, leanCurve.Evaluate(leftLeanAmount));
        Vector3 leftOffset = leftLean - cameraRestPosition;

        // --- Calculating Camera Position & Orientation ---
        Camera.main.transform.localPosition = cameraRestPosition + frontOffset + backOffset + rightOffset + leftOffset;

        Vector3 cameraLookTarget = Camera.main.transform.localPosition + transform.parent.forward;
        Vector3 cameraUp = (Camera.main.transform.position - transform.parent.position).normalized;
        cameraLookTarget = Vector3.RotateTowards(cameraLookTarget, cameraPitch > 0f ? Vector3.up : Vector3.down, Mathf.Abs(cameraPitch) * Mathf.Deg2Rad, 0f);
        Camera.main.transform.LookAt(transform.parent.position + cameraLookTarget, cameraUp);
        float upDifference = frontOffset.z + backOffset.z;
        if (frontOffset != Vector3.zero || backOffset != Vector3.zero)
        {
            //Camera.main.transform.Rotate(new Vector3(upDifference, 0f, 0f));
        }
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
