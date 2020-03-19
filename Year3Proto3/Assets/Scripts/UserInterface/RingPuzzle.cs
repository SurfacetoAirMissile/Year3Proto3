using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RingPuzzle : MonoBehaviour
{
    public Color unselectedColor;
    public Color selectedColor;
    public int ringCount;
    public int selectedIndex;
    public string keyCorrect;
    public string keyCurrent;

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
        public RotationState rotationState;
    }

    [SerializeField]
    private Rings[] ring;

    void Start()
    {
        ringCount = transform.Find("Rings").childCount;

        for (int i = 0; i < ringCount; i++)
        {
            ring[i].ringObject = transform.Find("Rings").GetChild(i).gameObject;
        }

        SetSelection();
    }


    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ring[selectedIndex].rotationState--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ring[selectedIndex].rotationState++;
        }
    }
}
