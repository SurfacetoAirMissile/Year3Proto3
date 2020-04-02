using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    private Vector3 initialPosition;
    private RingPuzzle finalPuzzle;
    public Vector3 Motion;
    private bool open = false;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void ToggleDoorOpen()
    {
        Vector3 position = new Vector3(0.0f, 2f, 0.0f);
        transform.DOKill(false);
        transform.DOLocalMove(open ? initialPosition : Motion + initialPosition, 1.2f)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => {
                open = !open;
            });
    }

    public bool isOpen()
    {
        return open;
    }
}

