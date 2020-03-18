using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerCamera playerCamera;
    public PlayerMovement playerMovement;

    private struct Lean
    {
        public Transform target;
        public float amount;
    }

    [SerializeField]
    private AnimationCurve leanCurve;
    private float leanSpeed = 3.0f;
    private Lean leftLean, rightLean, frontLean, backLean;
    private Transform restPosition;

    private void Awake()
    {
        restPosition = transform.Find("CameraRestPosition");

        for(int i = 1; i < transform.childCount; i ++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.Contains("FrontLeanTarget")) frontLean.target = child; 
            if (child.name.Contains("BackLeanTarget"))  backLean.target = child;
            if (child.name.Contains("RightLeanTarget")) rightLean.target = child;
            if (child.name.Contains("LeftLeanTarget")) leftLean.target = child;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        playerCamera.Refresh();
        playerMovement.Refresh();

        LeanTowards(UpdateLean(KeyCode.W, frontLean), UpdateLean(KeyCode.S, backLean));
        LeanTowards(UpdateLean(KeyCode.D, rightLean), UpdateLean(KeyCode.A, leftLean));
    }

    private Vector3 UpdateLean(KeyCode keyCode, Lean lean)
    {
        if (Input.GetKey(keyCode)) { lean.amount += Time.deltaTime * leanSpeed; }
        else { lean.amount -= Time.deltaTime * leanSpeed; }
        lean.amount = Mathf.Clamp(lean.amount, 0.0f, 1.0f);

        Transform cameraPosition = restPosition;

        Vector3 leanVector = Vector3.Lerp(
            cameraPosition.position,
            lean.target.position, 
            leanCurve.Evaluate(lean.amount)
        );

        return leanVector - cameraPosition.position;
    }

    private void LeanTowards(Vector3 a, Vector3 b)
    {
        float zDifference = a.z - b.z;
        if(a != Vector3.zero || b != Vector3.zero)
        {
            Camera.main.transform.Rotate(new Vector3(zDifference, 0.0f, 0.0f));
        }
    }
}
