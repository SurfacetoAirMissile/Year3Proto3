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

    public enum PuzzleID
    {
        ring,
        dot,
        random,
    }

    private List<GameObject> puzzles;

    public bool playerControl = true;
    //public GameObject ringPuzzle;

    public GameObject CreatePuzzle(PuzzleID _puzzle = PuzzleID.random)
    {
        if (_puzzle == PuzzleID.random)
        {
            return puzzles[Random.Range(0, puzzles.Count)];
        }
        else return puzzles[(int)_puzzle];
    }
    // Start is called before the first frame update
    void Start()
    {
        puzzles = new List<GameObject>();
        puzzles.Add(Resources.Load("Puzzles/RingPuzzle") as GameObject);
        puzzles.Add(Resources.Load("Puzzles/DotPuzzle") as GameObject);
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
