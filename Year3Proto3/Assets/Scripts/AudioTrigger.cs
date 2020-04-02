using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    BoxBehaviour box;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponentInParent<BoxBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent)
        {
            //Enemy enemy = other.transform.parent.GetComponent("Enemy") as Enemy;
            Enemy enemy = other.transform.parent.GetComponent<Enemy>();
            if (enemy)
            { 
                if (!box.listeners.Contains(enemy)) 
                { 
                    box.listeners.Add(enemy); 
                } 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent)
        {
            Enemy enemy = other.transform.parent.GetComponent("Enemy") as Enemy;
            if (enemy) 
            { 
                if (box.listeners.Contains(enemy)) 
                { 
                    box.listeners.Remove(enemy); 
                } 
            }
        }
    }
}
