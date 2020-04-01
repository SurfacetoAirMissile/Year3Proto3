using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EndScreen : MonoBehaviour
{
    public string[] startMessage;
    public float startMessageTime;
    private float startMessageTimer;
    private bool showStartMessage = true;
    private TMP_Text startText;

    void Start()
    {
        startText = transform.Find("StartText").GetComponent<TMP_Text>();

        if (GlobalData.deathCount < startMessage.Length)
        {
            startText.text = startMessage[GlobalData.deathCount];
        }
        else
        {
            startText.text = startMessage[startMessage.Length - 1];
        }

        Debug.Log(startText.text.Length);

        startMessageTimer = startMessageTime * (startText.text.Length * 0.01f);
    }


    void Update()
    {
        if (showStartMessage)
        {
            startMessageTimer -= Time.deltaTime;
        }

        if (startMessageTimer <= 0.0f && showStartMessage)
        {
            startText.GetComponent<CanvasGroup>().DOFade(0.0f, 2.0f);
            showStartMessage = false;
        }
    }
}
