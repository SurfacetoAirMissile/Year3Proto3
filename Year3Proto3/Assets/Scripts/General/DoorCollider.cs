using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    private bool enter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") enter = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") enter = false;
    }

    public bool HasEntered()
    {
        return enter;
    }
}
 
