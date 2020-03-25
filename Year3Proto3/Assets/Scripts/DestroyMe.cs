using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    [SerializeField]
    float lifetime = 0f;
    [SerializeField]
    bool active = true;



    public void SetLifetime(float _newLifetime)
    {
        lifetime = _newLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Prime()
    {
        active = true;
    }
}
