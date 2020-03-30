using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Interaction
{
    None,
    Generic,
    Hacking,
    Pickup,
    Holding,
    Puzzle
}

public class InteractionPrompt : MonoBehaviour
{
    public Interaction interactionState;
    private GameObject[] prompts;

    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 1.0f;

        prompts = new GameObject[transform.childCount];

        for (int i = 0; i < prompts.Length; i++)
        {
            prompts[i] = transform.GetChild(i).gameObject;
        }
    }


    void Update()
    {
        for (int i = 0; i < prompts.Length; i++)
        {
            if (i == (int)interactionState) { prompts[i].SetActive(true); }
            else { prompts[i].SetActive(false); }
        }
    }

    public void SetPrompt(Interaction _interaction)
    {
        interactionState = _interaction;
    }
}
