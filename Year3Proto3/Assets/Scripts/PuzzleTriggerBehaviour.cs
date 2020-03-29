using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTriggerBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool isEnemy;
    private Enemy enemy;
    // Start is called before the first frame update

    void Start()
    {
        if (isEnemy)
        {
            enemy = GetComponentInParent<Enemy>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<Enemy>().isActive())
        {
            if (other.gameObject.name.Contains("Player") && isEnemy)
            {
                if (other.GetComponentInChildren<PlayerController>().hackableEnemy == null)
                {
                    other.GetComponentInChildren<PlayerController>().hackableEnemy = enemy;
                    other.GetComponentInChildren<PlayerController>().puzzleDestination = transform.position;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Player") && isEnemy)
        {
            if (other.GetComponentInChildren<PlayerController>().hackableEnemy == enemy)
            { other.GetComponentInChildren<PlayerController>().hackableEnemy = null; }
        }
    }
}
