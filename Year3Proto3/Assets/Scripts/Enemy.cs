using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float angle;
    public float radius;

    public Transform target;
    public LayerMask layer;

    private bool seenTarget;

    private void Start()
    {
        StartCoroutine(TargetInRange());
    }

    IEnumerator TargetInRange()
    {
        while (true)
        {
            if (AwareOfTarget())
            {
                seenTarget = true;
            }
            else
            {
                seenTarget = false;
            }
            yield return new WaitForSeconds(5);
        }
    }

    private bool AwareOfTarget()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle >= (this.angle * 0.5f)) return false;

        if (Physics.Raycast(transform.position + transform.up, direction.normalized, out RaycastHit raycastHit, radius))
        {
            if (raycastHit.collider.gameObject.layer == layer) return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-(radius / 2.0f), transform.up) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(radius / 2.0f, transform.up) * transform.forward * radius);

        if (seenTarget) Gizmos.color = Color.red;

        Gizmos.DrawCube(target.position, new Vector3(1.01f, 1.01f, 1.01f));
    }
}
