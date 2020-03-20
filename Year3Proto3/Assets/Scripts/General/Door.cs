using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    public DoorCollider doorCollider;
     
    private Vector3 initialPosition;

    private bool open = false;
    private bool interact = true;

    private void Start()
    {
        initialPosition = transform.position;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && interact && doorCollider.HasEntered())
        {
            Vector3 position = new Vector3(0.0f, 0.0f, transform.localScale.z);

            interact = false;

            transform.DOKill(false);
            transform.DOLocalMove((open) ? initialPosition : (transform.rotation * position) + initialPosition, 1.2f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => {
                    open = (open) ? false : true;
                    interact = true;
                });
        }
    }
}
