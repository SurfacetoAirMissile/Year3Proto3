using TMPro;
using UnityEngine;

public class Framerate : MonoBehaviour
{
    private TMP_Text text;
    private float timer;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        timer -= Time.unscaledDeltaTime;

        if (timer <= 0)
        {
            text.text = (1.0f / Time.unscaledDeltaTime).ToString("0");
            timer = 0.25f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Complete is " + FindObjectOfType<RingPuzzle>().Validate());
        }
    }
}