using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleBehaviour : MonoBehaviour
{
    public Door door = null;
    public bool active = true;
    private bool colourChanged = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && !colourChanged)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", Color.green);
            transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", Color.green);
            colourChanged = true;
        }
    }
}
