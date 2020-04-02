using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    List<Enemy> listeners;
    bool muted = true;
    float muteTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        listeners = new List<Enemy>();
    }

    private void Update()
    {
        if (muted)
        {
            muteTime += Time.deltaTime;
            if (muteTime >= 1.5f)
            {
                muted = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.impulse / Time.fixedDeltaTime).magnitude > 12f && !muted)
        {
            GetComponent<AudioSource>().Play();
            foreach (Enemy enemy in listeners)
            {
                if (enemy.isActive()) enemy.InvestigateTarget(transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent)
        {
            Enemy enemy = other.transform.parent.GetComponent("Enemy") as Enemy;
            if (enemy) { if (!listeners.Contains(enemy)) { listeners.Add(enemy); } }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent)
        {
            Enemy enemy = other.transform.parent.GetComponent("Enemy") as Enemy;
            if (enemy) { if (listeners.Contains(enemy)) { listeners.Remove(enemy); } }
        }
    }
}
