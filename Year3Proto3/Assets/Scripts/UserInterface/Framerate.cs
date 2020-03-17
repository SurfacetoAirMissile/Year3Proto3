using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Framerate : MonoBehaviour
{
    private TMP_Text text;
    private float timer;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }


    void Update()
    {
        timer -= Time.unscaledDeltaTime;

        if (timer <= 0)
        {
            text.text = (1.0f / Time.unscaledDeltaTime).ToString("0");
            timer = 0.25f;
        }

        
    }
}
