using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    //private bool enter = false;
    private Door doorScript = null;

    private void Awake()
    {
        doorScript = transform.parent.GetComponent<Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Player") enter = true;
        if (!doorScript.isOpen())
        {
            if (other.gameObject.tag == "Player")
            {
                //enter = true;
                if (other.GetComponentInChildren<PlayerController>().hackableDoor != doorScript)
                {
                    other.GetComponentInChildren<PlayerController>().hackableDoor = doorScript;
                    other.GetComponentInChildren<PlayerController>().puzzleDestination = (other.transform.position - transform.position).normalized + transform.position;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!doorScript.isOpen())
        {
            if (other.gameObject.tag == "Player")
            {
                //enter = false;
                if (other.GetComponentInChildren<PlayerController>().hackableDoor == doorScript)
                {
                    other.GetComponentInChildren<PlayerController>().hackableDoor = null;
                }
            }
        }
    }

    /*
    public bool HasEntered()
    {
        return enter;
    }
    */
}
 
