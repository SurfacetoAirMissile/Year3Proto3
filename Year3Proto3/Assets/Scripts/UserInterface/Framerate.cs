using TMPro;
using UnityEngine;

public class Framerate : MonoBehaviour
{
    private TMP_Text text;
    private float timer;

    private void Start()
    {
        //text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        //timer -= Time.unscaledDeltaTime;

        //if (timer <= 0)
        //{
        //    text.text = (1.0f / Time.unscaledDeltaTime).ToString("0");
        //    timer = 0.25f;
        //}

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.None);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Hacking);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Pickup);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Holding);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FindObjectOfType<InteractionPrompt>().SetPrompt(Interaction.Puzzle);
        }

    }
}