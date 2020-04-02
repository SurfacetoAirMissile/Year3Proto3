using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTriggerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;
    private Transform lookPoint;
    // Start is called before the first frame update

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        lookPoint = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<Enemy>().isActive())
        {
            if (other.gameObject.name.Contains("Player"))
            {
                if (other.GetComponentInChildren<PlayerController>().hackableEnemy == null)
                {
                    other.GetComponentInChildren<PlayerController>().hackableEnemy = enemy;
                    other.GetComponentInChildren<PlayerController>().puzzleDestination = transform.position;
                    other.GetComponentInChildren<PlayerController>().puzzleLookDestination = lookPoint.position;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (GetComponentInParent<Enemy>().isActive())
        {
            if (other.gameObject.name.Contains("Player"))
            {
                if (other.GetComponentInChildren<PlayerController>().hackableEnemy == enemy)
                {
                    other.GetComponentInChildren<PlayerController>().puzzleDestination = transform.position;
                    other.GetComponentInChildren<PlayerController>().puzzleLookDestination = lookPoint.position;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
        {
            if (other.GetComponentInChildren<PlayerController>().hackableEnemy == enemy)
            { other.GetComponentInChildren<PlayerController>().hackableEnemy = null; }
        }
    }
}
