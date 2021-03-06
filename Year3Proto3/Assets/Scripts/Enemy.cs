﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Enemy : MonoBehaviour
{

    [Serializable]
    public enum AIState
    {
        watch,
        patrol,
        investigate,
        deactivated
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
        public Watch point;
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
    [SerializeField] [Tooltip("If the enemy is set to be patrol, this object needs to be filled. The GameObject must have a series of points as child GameObjects, each point's position will be taken as a point to patrol, and it's Y scale will be taken as how long to stay at that point for. The object will be destroyed at runtime.")]
    private Transform startPath = null;

    // --- MUST BE INITIALIZED IN START ---
    private Transform player;
    private NavMeshAgent agent;
    private AIState currentState;
    private AIState previousState;
    private StateObject currentStateObject;
    private bool lightsActive;
    //private bool seenTarget;
    private int currentPoint;
    private float currentPointTime;
    public bool isBeingHacked;
    private Vector3 investigationTarget;
    private bool canGetToInvestigateTarget;
    private float investigateTimer;
    private float investigationTime;
    private float activeEmissionIntensity = 0f;
    private Rigidbody enemyRB;
    private bool animWalking = false;
    private Animator enemyAnim = null;

    private void Start()
    {
        isBeingHacked = false;
        currentState = startState;
        currentStateObject = startStateObject;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        if (startState == AIState.patrol)
        {
            currentStateObject.patrol = ConvertPathObject(startPath);
            agent.SetDestination(currentStateObject.patrol.points[currentPoint].point.standPosition);
        }
        else
        {
            agent.SetDestination(currentStateObject.guard.standPosition);
        }
        //agent.isStopped = true;
        lightsActive = true;
        currentPointTime = 0f;
        currentPoint = 0;
        investigationTarget = Vector3.zero;
        canGetToInvestigateTarget = false;
        investigateTimer = 0f;
        investigationTime = 5f;
        activeEmissionIntensity = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.GetFloat("Vector1_E770BCCD");
        enemyRB = GetComponent<Rigidbody>();
        enemyAnim = GetComponent<Animator>();
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

    public void InvestigateTarget(Vector3 _Target)
    {
        if (currentState != AIState.investigate)
        {
            previousState = currentState;
            SwitchState(AIState.investigate);
        }
        NavMeshPath path = new NavMeshPath();
        investigationTarget = _Target;
        investigateTimer = 0f;
        if (NavMesh.SamplePosition(_Target, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            canGetToInvestigateTarget = agent.CalculatePath(hit.position, path);
            if (canGetToInvestigateTarget)
            {
                agent.SetDestination(_Target);
            }
        }
    }

    private bool AwareOfPlayer()
    {
        Vector3 eyePosition = transform.Find("Root/Hips/Spine1/Spine2/Spine3/Neck1/Neck2/Head/Spot Light").position;
        Vector3 directionToFeet = player.position - eyePosition;
        float angleToFeet = Vector3.Angle(directionToFeet, transform.forward);
        bool withinMaxAngleFeet = angleToFeet <= (FOV * 0.5f);
        bool hitPlayerLayerFeet = false;
        if (Physics.Raycast(eyePosition, directionToFeet.normalized, out RaycastHit raycastHit, spottingDistance))
        {
            hitPlayerLayerFeet = raycastHit.collider.gameObject.layer == kPlayerLayer;
        }

        Vector3 directionToHead = player.GetChild(0).position - eyePosition;
        float angleToHead = Vector3.Angle(directionToHead, transform.forward);
        bool withinMaxAngleHead = angleToHead <= (FOV * 0.5f);
        bool hitPlayerLayerHead = false;
        if (Physics.Raycast(eyePosition, directionToHead.normalized, out raycastHit, spottingDistance))
        {
            hitPlayerLayerHead = raycastHit.collider.gameObject.layer == kPlayerLayer;
        }

        return (hitPlayerLayerFeet && withinMaxAngleFeet) || (hitPlayerLayerHead && withinMaxAngleHead);
    }

    private void FixedUpdate()
    {
        //agent.nextPosition = transform.position;
        if (!isBeingHacked)
        {
            switch (currentState)
            {
                case AIState.watch:
                    CheckPlayerSpotted();
                    GuardUpdate();
                    break;
                case AIState.patrol:
                    CheckPlayerSpotted();
                    PatrolUpdate();
                    break;
                case AIState.investigate:
                    CheckPlayerSpotted();
                    InvestigateUpdate();
                    break;
                case AIState.deactivated:
                    break;
            }
        }
        else
        {
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            animWalking = false;
        }
        enemyAnim.SetBool("Walking", animWalking);
    }

    void GuardUpdate()
    {
        float distance = Vector3.Distance(transform.position, currentStateObject.guard.standPosition);
        // If the point that the enemy needs to stand at is further than 0.25m away...
        if (distance > 0.25f)
        {
            // Move the Enemy towards where the NavMesh wants to go.
            Vector3 steeringDirection = agent.steeringTarget;
            steeringDirection.y = transform.position.y;
            Vector3 difference = (steeringDirection - transform.position) * 4f;
            Vector3 forceVector = difference.magnitude > 1 ? difference.normalized : difference;
            //GetComponent<Rigidbody>().AddForce(forceVector * movementSpeed);

            // Look where you are going.
            Vector3 lookTarget = agent.steeringTarget;
            lookTarget.y = transform.position.y;
            Vector3 lookDirection = lookTarget - transform.position;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 250 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
            agent.isStopped = false;
            animWalking = true;
        }
        else // If the point is not 0.25m away...
        {
            // Rotate the guard towards the position they need to watch.
            Vector3 lookTarget = currentStateObject.guard.watchPosition;
            lookTarget.y = transform.position.y;
            Vector3 lookDirection = lookTarget - transform.position;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 250 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
            agent.isStopped = true;
            animWalking = false;
        }
    }

    void PatrolUpdate()
    {
        // GoToCurrentPathPoint returns true if the enemy is within the minimum distance for a point
        currentPointTime += Time.deltaTime;
        if (currentPointTime >= currentStateObject.patrol.points[currentPoint].waitTime)
        {
            currentPointTime = 0f;
            currentPoint = GetNextPathPoint();
            agent.SetDestination(currentStateObject.patrol.points[currentPoint].point.standPosition);
        }
        else
        {
            GoToCurrentPathPoint();
        }
    }

    void InvestigateUpdate()
    {
        if (investigateTimer < investigationTime)
        {
            if (canGetToInvestigateTarget)
            {
                float distance = Vector3.Distance(transform.position, agent.destination);
                // If the point that the enemy needs to stand at is further than 0.25m away...
                if (distance > 2f)
                {
                    // Move the Enemy towards where the NavMesh wants to go.
                    //Vector3 steeringDirection = agent.steeringTarget;
                    //Vector3 difference = (steeringDirection - transform.position) * 4f;
                    //Vector3 forceVector = difference.magnitude > 1 ? difference.normalized : difference;
                    //Vector3 forceVector = difference.normalized;
                    //GetComponent<Rigidbody>().AddForce(forceVector * movementSpeed);

                    // Look where you are going.
                    //Vector3 lookTarget = agent.steeringTarget;
                    //lookTarget.y = transform.position.y;
                    //Vector3 lookDirection = lookTarget - transform.position;
                    //transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 250 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                    investigateTimer += Time.fixedDeltaTime;
                    agent.isStopped = false;
                    animWalking = true;
                }
                else // If the point is not 2m away...
                {
                    // Rotate the guard towards the position they need to watch.
                    Vector3 lookTarget = agent.destination;
                    lookTarget.y = transform.position.y;
                    Vector3 lookDirection = lookTarget - transform.position;
                    transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 250 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                    investigateTimer += Time.fixedDeltaTime;
                    agent.isStopped = true;
                    animWalking = false;
                }
            }
            else
            {
                // Rotate the guard towards the position they need to watch.
                Vector3 lookTarget = investigationTarget;
                lookTarget.y = transform.position.y;
                Vector3 lookDirection = lookTarget - transform.position;
                transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 250 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                investigateTimer += Time.fixedDeltaTime;
                agent.isStopped = true;
            }
        }
        else
        {
            SwitchState(previousState);
        }
    }

    void CheckPlayerSpotted()
    {
        // If the enemy is aware of the player...
        if (AwareOfPlayer())
        {
            // Set the colours.
            ActivateLights(spottedColour);
            AudioSource audio = GetComponentInChildren<AudioSource>();
            if (!audio.isPlaying) { audio.Play(); }
            FindObjectOfType<SceneSwitcher>().DeathFade();
        }
        else // If the enemy isn't aware of the player...
        {
            // Set the colours.
            ActivateLights(normalColour);
        }
    }

    void DeactivateLights()
    {
        // Set the colours and switch off the light.
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_538469C8", Color.black);
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetFloat("Vector1_E770BCCD", 0f);
        transform.Find("Root/Hips/Spine1/Spine2/Spine3/Neck1/Neck2/Head/Spot Light").GetComponent<Light>().enabled = false;
        lightsActive = false;
    }

    void ActivateLights(Color _colour)
    {
        // Set the colours and switch on the light.
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_538469C8", _colour);
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetFloat("Vector1_E770BCCD", activeEmissionIntensity);
        transform.Find("Root/Hips/Spine1/Spine2/Spine3/Neck1/Neck2/Head/Spot Light").GetComponent<Light>().enabled = true;
        transform.Find("Root/Hips/Spine1/Spine2/Spine3/Neck1/Neck2/Head/Spot Light").GetComponent<Light>().color = _colour;
        lightsActive = true;
    }

    Path ConvertPathObject(Transform _pathObject)
    {
        Path returnPath;
        returnPath.points = new List<PathPoint>();
        for (int i = 0; i < _pathObject.childCount; i++)
        {
            PathPoint newPathPoint = new PathPoint();
            newPathPoint.point.standPosition = _pathObject.GetChild(i).position;
            newPathPoint.point.watchPosition = _pathObject.GetChild(i).GetChild(0).position;
            newPathPoint.waitTime = _pathObject.GetChild(i).localScale.y;
            returnPath.points.Add(newPathPoint);
        }
        Destroy(_pathObject.gameObject);
        return returnPath;
    }

    int GetNextPathPoint()
    {
        if (currentStateObject.patrol.points.Count == currentPoint + 1)
        { return 0; }
        else return currentPoint + 1;
    }

    public void SwitchState(AIState _newState)
    {
        currentState = _newState;
        switch (_newState)
        {
            case AIState.watch:
                agent.SetDestination(currentStateObject.guard.standPosition);
                break;
            case AIState.patrol:
                agent.SetDestination(currentStateObject.patrol.points[currentPoint].point.standPosition);
                break;
            case AIState.investigate:
                break;
            case AIState.deactivated:
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                if (lightsActive) { DeactivateLights(); }
                break;
            default:
                break;
        }
    }

    bool GoToCurrentPathPoint()
    {
        Vector3 target = currentStateObject.patrol.points[currentPoint].point.standPosition;
        target.y = transform.position.y;
        float distance = Vector3.Distance(transform.position, target);
        // If the point that the enemy needs to stand at is further than 0.25m away...
        if (distance > 0.25f)
        {
            // Rotate the guard towards the position they need to watch.
            //Vector3 lookTarget = currentStateObject.patrol.points[currentPoint].point;
            //lookTarget.y = transform.position.y;
            //Vector3 lookDirection = lookTarget - transform.position;
            //transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 5 * Mathf.Deg2Rad, 0f);

            // Move the Enemy towards where the NavMesh wants to go.
            //Vector3 steeringDirection = agent.steeringTarget;
            //Vector3 forceVector = (steeringDirection - transform.position).normalized;
            //GetComponent<Rigidbody>().AddForce(forceVector * movementSpeed);
            agent.isStopped = false;
            animWalking = true;
            return false;
        }
        else
        {
            Vector3 lookTarget = currentStateObject.patrol.points[currentPoint].point.watchPosition;
            lookTarget.y = transform.position.y;
            Vector3 lookDirection = lookTarget - transform.position;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, 5 * Mathf.Deg2Rad, 0f);
            agent.isStopped = true;
            animWalking = false;
            return true;
        }
    }

    public bool isActive()
    {
        return currentState != AIState.deactivated;
    }
}
