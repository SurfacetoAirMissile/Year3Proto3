using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RingPuzzle : Puzzle
{
    public Color unselectedColor;
    public Color selectedColor;
    public Color completeColor;
    public int selectedIndex;
    public string keyCorrect;
    public string keyCurrent;

    private int stateCount;
    private int ringCount;
    private bool hasInitialized;

    [System.Serializable]
    public struct Rings
    {
        public GameObject ringObject;
        public bool isSelected;

        public enum RotationState
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        }

        public RotationState rotationStateInitial;
        public RotationState rotationState;
    }

    [SerializeField]
    private Rings[] ring;

    private void Start()
    {
        stateCount = System.Enum.GetValues(typeof(Rings.RotationState)).Length;
        ringCount = transform.Find("Rings").childCount;

        for (int i = 0; i < ringCount; i++)
        {
            ring[i].ringObject = transform.Find("Rings").GetChild(i).gameObject;
        }

        SetSelection();

        InitializePuzzle();
    }

    private void Update()
    {
        // Selecting
        if (Input.GetKeyDown(KeyCode.UpArrow) && selectedIndex > 0)
        {
            selectedIndex--;
            SetSelection();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && selectedIndex < ringCount - 1)
        {
            selectedIndex++;
            SetSelection();
        }

        // Rotating
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ring[selectedIndex].rotationState--;
            if (ring[selectedIndex].rotationState < 0)
            {
                ring[selectedIndex].rotationState = (Rings.RotationState)stateCount - 1;
            }
            SetRotation();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ring[selectedIndex].rotationState++;
            if (ring[selectedIndex].rotationState > (Rings.RotationState)stateCount - 1)
            {
                ring[selectedIndex].rotationState = 0;
            }
            SetRotation();
        }
    }

    private void InitializePuzzle()
    {
        // Store initial rotation state of rings
        if (!hasInitialized)
        {
            for (int i = 0; i < ringCount; i++)
            {
                ring[i].rotationStateInitial = (Rings.RotationState)Mathf.RoundToInt(ring[i].ringObject.transform.localRotation.eulerAngles.z / 45.0f);
                ring[i].rotationState = ring[i].rotationStateInitial;

                hasInitialized = true;
            }
        }

        // Scramble rotation states of rings
        for (int i = 0; i < ringCount; i++)
        {
            while (ring[i].rotationState == ring[i].rotationStateInitial)
            {
                ring[i].rotationState = (Rings.RotationState)Random.Range(0, stateCount - 1);
            }
        }

        SetRotation();
    }

    private void SetSelection()
    {
        for (int i = 0; i < ringCount; i++)
        {
            if (i == selectedIndex)
            {
                ring[i].ringObject.GetComponent<Image>().color = selectedColor;
                ring[i].ringObject.transform.DOKill(true);
                ring[i].ringObject.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.0f), 0.15f, 1, 1.0f);

                ring[i].isSelected = true;
            }
            else
            {
                ring[i].ringObject.GetComponent<Image>().color = unselectedColor;

                ring[i].isSelected = false;
            }
        }
    }

    private void SetRotation()
    {
        for (int i = 0; i < ringCount; i++)
        {
            float targerRot = (float)ring[i].rotationState * 45.0f;
            ring[i].ringObject.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, targerRot), 0.3f).SetEase(Ease.OutQuint);
        }

        SetValidation();
    }

    private void SetValidation()
    {
        bool tempValid = true;

        for (int i = 0; i < ringCount; i++)
        {
            if (ring[i].rotationState != ring[i].rotationStateInitial)
            {
                tempValid = false;
            }
        }

        isComplete = tempValid;

        transform.Find("Check").GetComponent<HologramFX>().showHologram = isComplete;
    }
}