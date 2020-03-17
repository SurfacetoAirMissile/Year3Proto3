using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyLight : MonoBehaviour
{
    private Light light;
    private float intensity;
    [Range(0, 1)] public float flickerAmount;

    void Start()
    {
        light = GetComponent<Light>();
        intensity = light.intensity;
    }


    void FixedUpdate()
    {
        float flickerAdd = Random.Range(-intensity, intensity) * flickerAmount;

        light.intensity = intensity + flickerAdd;
    }
}
