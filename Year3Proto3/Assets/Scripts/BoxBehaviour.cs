using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    List<Enemy> listeners;

    // Start is called before the first frame update
    void Start()
    {
        listeners = new List<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.impulse / Time.fixedDeltaTime).magnitude > 4f)
        {
            GetComponent<AudioSource>().Play();
            foreach (Enemy enemy in listeners)
            {
                if (enemy.isActive())
                {
                    enemy.InvestigateTarget(transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent("Enemy") as Enemy;
        if (enemy) { if (!listeners.Contains(enemy)) { listeners.Add(enemy); } }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent("Enemy") as Enemy;
        if (enemy) { if (listeners.Contains(enemy)) { listeners.Remove(enemy); } }
    }
}
