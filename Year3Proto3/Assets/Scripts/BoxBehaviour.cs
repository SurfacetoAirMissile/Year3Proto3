using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    public List<Enemy> listeners;
    bool muted = true;
    float muteTime = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        listeners = new List<Enemy>();
    }

    private void Update()
    {
        if (muted)
        {
            muteTime += Time.deltaTime;
            if (muteTime >= 1.5f)
            {
                muted = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.impulse / Time.fixedDeltaTime).magnitude > 12f && !muted)
        {
            transform.Find("Audio").GetComponent<AudioSource>().Play();
            foreach (Enemy enemy in listeners)
            {
                if (enemy.isActive()) enemy.InvestigateTarget(transform.position);
            }
        }
    }
}
