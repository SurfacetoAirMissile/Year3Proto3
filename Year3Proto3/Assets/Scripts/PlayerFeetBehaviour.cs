using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeetBehaviour : MonoBehaviour
{
    SphereCollider sphere = null;
    Rigidbody rb = null;
    float stepDelay = 0.10f;
    float lastStep = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        sphere = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        lastStep += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Stairs")
        {
            float difference = collision.collider.bounds.max.y - sphere.bounds.min.y;
            //if (difference > 0f && lastStep > stepDelay)
            if (difference > 0f)
            {
                Vector3 direction = collision.transform.position - transform.position;
                direction.y *= -1;
                direction.Normalize();
                Vector3 tempPos = transform.position;
                //tempPos.y += difference;
                tempPos += direction * 1.5f * difference;
                transform.position = tempPos;
                lastStep = 0f;
                rb.velocity = Vector3.zero;
            }
        }
    }
}
