using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensitivity = 90f;

    private float pitch;

    public void Refresh()
    {
        float mouseX = Mathf.Clamp(Input.GetAxisRaw("Mouse X") * sensitivity * Time.smoothDeltaTime, -50f, 50f);
        float mouseY = Mathf.Clamp(Input.GetAxisRaw("Mouse Y") * sensitivity * Time.smoothDeltaTime, -50f, 50f);

        pitch += mouseY;
        pitch = Mathf.Clamp(pitch, -45.0f, 45.0f);

        transform.parent.Rotate(new Vector3(0f, mouseX, 0f));

        Vector3 cameraLook = transform.parent.forward;
        cameraLook = Vector3.RotateTowards(cameraLook, pitch > 0f ? Vector3.up : Vector3.down, Mathf.Abs(pitch) * Mathf.Deg2Rad, 0f);
        Camera.main.transform.LookAt(transform.parent.position + Camera.main.transform.localPosition + cameraLook);
    }
}
