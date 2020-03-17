using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyLight : MonoBehaviour
{
    private Light light;
    private float intensity;
    [Range(0, 1)] public float flickerAmount;
    public float flickerRateMin = 10.0f;
    public float flickerRateMax = 30.0f;
    private float flickerTimer;

    void Start()
    {
        light = GetComponent<Light>();
        intensity = light.intensity;
        flickerTimer = 1.0f / Random.Range(flickerRateMin, flickerRateMax);
    }


    private void Update()
    {
        flickerTimer -= Time.deltaTime;

        if (flickerTimer <= 0.0f)
        {
            SetFlicker();
            flickerTimer = 1.0f / Random.Range(flickerRateMin, flickerRateMax);
        }
    }

    void FixedUpdate()
    {

    }

    private void SetFlicker()
    {
        float flickerAdd = Random.Range(-intensity, intensity) * flickerAmount;

        light.intensity = intensity + flickerAdd;
    }
}
