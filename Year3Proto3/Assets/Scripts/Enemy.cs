using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Enemy : MonoBehaviour
{

    [Serializable]
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
        public List<PathPoint> points;
    }

    [Serializable]
    struct PathPoint
    {
        public Vector3 point;
        public float waitTime;
    }

    [Serializable]
    struct Watch
    {
        public Vector3 standPosition;
        public Vector3 watchPosition;
    }

    // --- HARD-CODED CONSTANTS ---
    private int kPlayerLayer = 9;

    // --- SET IN EDITOR ---
    [SerializeField] [Tooltip("The enemy's field of view.")]
    private float FOV = 60f;
    [SerializeField] [Tooltip("The maximum distance that the enemy can see.")]
    private float spottingDistance = 40f;
    [SerializeField] [Tooltip("How quickly the enemy moves.")]
    private float movementSpeed = 1.5f;
    [SerializeField] [Tooltip("The colour of the enemy's light when the enemy hasn't spotted the player.")]
    private Color normalColour = Color.blue;
    [SerializeField] [Tooltip("The colour of the enemy's light when the enemy has spotted the player.")]
    private Color spottedColour = Color.red;
    [SerializeField] [Tooltip("The enemies starting state.")]
    private AIState startState = AIState.watch;
    [SerializeField] [Tooltip("The enemies starting stateObject.")]
    private StateObject startStateObject = new StateObject();

    // --- MUST BE INITIALIZED IN AWAKE ---
    private Transform player;
    private NavMeshAgent agent;
    private AIState currentState;
    private StateObject currentStateObject;
    //private bool seenTarget;

    private void Start()
    {
        currentState = startState;
        currentStateObject = startStateObject;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.SetDestination(currentStateObject.guard.standPosition);
        agent.isStopped = true;
    }

    /*
    IEnumerator TargetInRange()
    {
        while (true)
        {
            if (AwareOfPlayer())
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
    */

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

    private bool AwareOfPlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle >= (FOV * 0.5f)) return false;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit raycastHit, spottingDistance))
        {
            if (raycastHit.collider.gameObject.layer == kPlayerLayer) return true;
        }
        return false;
    }

    private void Update()
    {
        agent.nextPosition = transform.position;
        CheckPlayerSpotted();
        switch (currentState)
        {
            case AIState.watch:
                GuardUpdate();
                break;
            case AIState.patrol:
                break;
        }

    }

    void GuardUpdate()
    {
        float distance = (transform.position - currentStateObject.guard.standPosition).magnitude;
        if (distance > 0.25f)
        {
            Vector3 steeringDirection = agent.steeringTarget;
            steeringDirection.y = transform.position.y;
            Vector3 difference = (steeringDirection - transform.position) * 4f;
            Vector3 forceVector = difference.magnitude > 1 ? difference.normalized : difference;
            GetComponent<Rigidbody>().AddForce(forceVector * Time.deltaTime * 1000f * movementSpeed);
        }
        else
        {
            Vector3 lookTarget = currentStateObject.guard.watchPosition;
            lookTarget.y = transform.position.y;
            Vector3 lookDirection = lookTarget - transform.position;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 5 * Mathf.Deg2Rad, 0f);
        }
    }

    void CheckPlayerSpotted()
    {
        if (AwareOfPlayer())
        {
            transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", spottedColour);
            transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", spottedColour);
            transform.GetChild(0).GetChild(1).GetComponent<Light>().color = spottedColour;
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", normalColour);
            transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", normalColour);
            transform.GetChild(0).GetChild(1).GetComponent<Light>().color = normalColour;
        }
    }
}
