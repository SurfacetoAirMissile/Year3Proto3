using UnityEngine;

public class EnemyLight : MonoBehaviour
{
    private Light bulb;
    private float intensity;
    [Range(0, 1)] public float flickerAmount;
    public float flickerRateMin = 10.0f;
    public float flickerRateMax = 30.0f;
    private float flickerTimer;

    private void Start()
    {
        bulb = GetComponent<Light>();
        intensity = bulb.intensity;
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

    private void SetFlicker()
    {
        float flickerAdd = Random.Range(-intensity, intensity) * flickerAmount;

        bulb.intensity = intensity + flickerAdd;
    }
}