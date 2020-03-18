using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

enum AIState
{
    watch,
    patrol
}

[Serializable]
struct StateObject
{
    public Path patrol;
    public Watch guard;
}

[Serializable]
struct Path
{
    public List<PathPoint> pathPoints;

    Path(List<PathPoint> _pathPoints)
    {
        pathPoints = _pathPoints;
    }
}

[Serializable]
struct PathPoint
{
    public Vector3 point;
    public float waitTime;

    PathPoint(Vector3 _point, float _waitTime)
    {
        point = _point;
        waitTime = _waitTime;
    }
}

[Serializable]
struct Watch
{
    public Vector3 standPosition;
    public Vector3 watchPosition;

    Watch(Vector3 _standPosition, Vector3 _watchPosition)
    {
        standPosition = _standPosition;
        watchPosition = _watchPosition;
    }
}

public class Enemy : MonoBehaviour
{
    public float angle;
    public float radius;

    public Transform target = null;
    public int layer = 0;

    private bool seenTarget;
    NavMeshAgent agent;
    AIState currentState = AIState.watch;
    StateObject currentStateObject;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //StartCoroutine(TargetInRange());
        agent.updatePosition = false;
        currentStateObject = new StateObject();
        currentStateObject.guard.watchPosition = new Vector3(-8f, 1f, -3f);
        currentStateObject.guard.standPosition = new Vector3(2f, 0.5f, -3f);
        agent.SetDestination(currentStateObject.guard.standPosition);
        target = GameObject.Find("Player").transform;
        layer = LayerMask.NameToLayer("Target");
        radius = 5f;
        angle = 60f;
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

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit raycastHit, radius))
        {
            if (raycastHit.collider.gameObject.layer == layer) return true;
        }
        return false;
    }

    private void Update()
    {
        agent.nextPosition = transform.position;

        switch (currentState)
        {
            case AIState.watch:
                GuardUpdate();
                break;
            case AIState.patrol:
                break;
        }

    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-(radius / 2.0f), transform.up) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(radius / 2.0f, transform.up) * transform.forward * radius);

        if (seenTarget) Gizmos.color = Color.red;

        Gizmos.DrawCube(target.position, new Vector3(1.01f, 1.01f, 1.01f));
    }
    */

    void GuardUpdate()
    {
        float distance = (transform.position - currentStateObject.guard.standPosition).magnitude;
        if (distance > 0.1f)
        {
            Vector3 steeringDirection = agent.steeringTarget;
            steeringDirection.y = transform.position.y;
            //Debug.DrawRay(transform.position, (steeringDirection - transform.position).normalized);
            //agent.Move((steeringDirection - transform.position).normalized * 10f);
            agent.isStopped = true;
            GetComponent<Rigidbody>().AddForce((steeringDirection - transform.position).normalized * 20f);
            agent.isStopped = false;
        }
        else
        {
            Vector3 lookTarget = currentStateObject.guard.watchPosition;
            lookTarget.y = transform.position.y;
            Vector3 lookDirection = lookTarget - transform.position;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 5 * Mathf.Deg2Rad, 0f);
            //transform.LookAt(lookTarget);
            if (AwareOfTarget())
            {
                transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", Color.red);
                transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", Color.red);
                transform.GetChild(0).GetChild(1).GetComponent<Light>().color = Color.red;
            }
        }
    }
}
