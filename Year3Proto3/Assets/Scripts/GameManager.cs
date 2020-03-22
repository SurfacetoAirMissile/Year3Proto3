using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); }
        else { instance = this; }
    }

    /*
    public GameObject CreatePuzzle()
    {

    }
    */

    public bool playerControl = true;
    public GameObject ringPuzzle;

    // Start is called before the first frame update
    void Start()
    {
        ringPuzzle = Resources.Load("Puzzles/RingPuzzle") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.P))
        {
            playerControl = !playerControl;
        }*/
    }
}
