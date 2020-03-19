using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RingPuzzle : Puzzle
{
    public bool multiMode;
    public Color unselectedColor;
    public Color selectedColor;
    public int selectedIndex;

    private int stateCount;
    public int ringCount;

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

        public RotationState rotationStateKey;
        public RotationState rotationState;
    }

    [SerializeField]
    private Rings[] ring;
    private Rings masterRing;

    private void Start()
    {
        stateCount = System.Enum.GetValues(typeof(Rings.RotationState)).Length;
        ringCount = transform.Find("Rings").childCount;

        masterRing.ringObject = transform.Find("MasterRing").gameObject;

        for (int i = 0; i < ringCount; i++)
        {
            ring[i].ringObject = transform.Find("Rings").GetChild(i).gameObject;
        }

        SetSelection(selectedIndex);
        InitializePuzzle();
    }

    private void Update()
    {
        // Change selection with up and down arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetSelection(selectedIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetSelection(selectedIndex + 1);
        }

        // Adjust rotation with left and right arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetRotation(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetRotation(1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            InitializePuzzle();
        }
    }

    private void InitializePuzzle()
    {
        // Scramble roatation of master ring
        masterRing.rotationState = (Rings.RotationState)Random.Range(0, stateCount - 1);
        masterRing.ringObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, (float)masterRing.rotationState * 45.0f);

        for (int i = 0; i < ringCount; i++)
        {
            // Store correct key rotation state of rings
            ring[i].rotationStateKey = masterRing.rotationState;

            // Scramble rotation states of rings
            ring[i].rotationState = (Rings.RotationState)Random.Range(0, stateCount - 1);
            // Do not allow any ring to be already in correct rotation
            while (ring[i].rotationState == ring[i].rotationStateKey)
            {
                ring[i].rotationState = (Rings.RotationState)Random.Range(0, stateCount - 1);
            }
        }

        SetRotation(0);
    }

    private void SetSelection(int index)
    {
        index = Mathf.Clamp(index, 0, ringCount - 1);
        selectedIndex = index;

        for (int i = 0; i < ringCount; i++)
        {
            bool multi = multiMode ? (i <= selectedIndex) : (i == selectedIndex);

            if (multi)
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

    public void SetRotation(int rot)
    {
        for (int i = 0; i < ringCount; i++)
        {
            // Rotate all rings that are selected
            if (ring[i].isSelected)
            {
                ring[i].rotationState += rot;
            }

            // Wrap around
            if (ring[i].rotationState < 0)
            {
                ring[i].rotationState = (Rings.RotationState)stateCount - 1;
            }
            if (ring[i].rotationState > (Rings.RotationState)stateCount - 1)
            {
                ring[i].rotationState = 0;
            }

            // Tween to rotation
            float targerRot = (float)ring[i].rotationState * 45.0f;
            ring[i].ringObject.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, targerRot), 0.3f).SetEase(Ease.OutQuint);
        }

        SetValidation();
    }

    private void SetValidation()
    {
        // Update completion state of puzzle

        bool tempValid = true;

        for (int i = 0; i < ringCount; i++)
        {
            if (ring[i].rotationState != ring[i].rotationStateKey)
            {
                tempValid = false;
            }
        }

        isComplete = tempValid;

        transform.Find("Check").GetComponent<HologramFX>().showHologram = isComplete;
    }
}