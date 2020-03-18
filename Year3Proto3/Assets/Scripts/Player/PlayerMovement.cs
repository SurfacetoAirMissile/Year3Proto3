using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public float jumpHeight = 10.0f;

    public new Rigidbody rigidbody;

    private bool grounded = true;

    public void Refresh()
    {
        Vector3 force = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) force += transform.parent.forward;
        if (Input.GetKey(KeyCode.S)) force -= transform.parent.forward;
        if (Input.GetKey(KeyCode.D)) force += transform.parent.right;
        if (Input.GetKey(KeyCode.A)) force -= transform.parent.right;

        if (force != Vector3.zero) rigidbody.AddForce(force.normalized * speed * 1000 * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rigidbody.AddForce(new Vector3(0.0f, jumpHeight, 0.0f), ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 1 << LayerMask.NameToLayer("Ground")) grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 1 << LayerMask.NameToLayer("Ground")) grounded = false;
    }
}
