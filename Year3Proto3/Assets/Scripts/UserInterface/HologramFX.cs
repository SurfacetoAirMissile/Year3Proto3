using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HologramFX : MonoBehaviour
{
    [Range(0.0f, 0.5f)] public float flickerAmount;
    private Image tex;

    void Start()
    {
        tex = GetComponent<Image>();
    }


    void FixedUpdate()
    {
        Vector4 col = tex.color;
        float alpha = 0.5f + (Random.Range(-flickerAmount, flickerAmount));
        col.w = alpha;

        tex.color = col;
    }
}
