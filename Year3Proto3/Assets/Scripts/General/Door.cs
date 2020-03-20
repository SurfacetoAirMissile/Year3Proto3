using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    public DoorCollider doorCollider;

    public Vector3 finalPosition;
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
            interact = false;

            transform.DOKill(false);
            transform.DOLocalMove((open) ? initialPosition : finalPosition, 1.2f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => {
                    open = (open) ? false : true;
                    interact = true;
                });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(finalPosition, transform.localScale);
    }
}
