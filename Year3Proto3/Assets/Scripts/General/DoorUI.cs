using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUI : MonoBehaviour
{
    public GameObject prompt;
    public Transform target;

    private bool range = false;

    private void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        float distanceFromObject = Vector3.Distance(Camera.main.transform.position, target.position);

        prompt.SetActive(screenPoint.z > 0.0f && distanceFromObject < (15.0f * transform.localScale.x));
    }
}
