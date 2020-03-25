using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DotPuzzle : Puzzle
{
    public Color unselectedColor;
    public Color selectedColor;

    public Sprite dotEmpty;
    public Sprite dotFull;

    private int selectedIndex;
    private bool waitDone;

    [System.Serializable]
    public struct Box
    {
        public GameObject boxObject;
        public bool isSelected;

        public int activeDots;
        public int finalDots;
    }

    [SerializeField]
    private Box[] boxes;
    private Box masterBox;

    private HologramFX holo;

    private void Start()
    {
        holo = GetComponent<HologramFX>();
        holo.showHologram = true;
        waitDone = false;
        InitializePuzzle();
    }

    private void Update()
    {
        if (holo.showHologram && !GameManager.Instance.playerControl && waitDone)
        {
            // Change selection with up and down arrow keys
            if (Input.GetKeyDown(KeyCode.UpArrow)) SetSelection(selectedIndex - 1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) SetSelection(selectedIndex + 1);


            // Adjust dot amount with left and right arrow keys
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SetDot(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) SetDot(1);


            if (Input.GetKeyDown(KeyCode.R)) InitializePuzzle();
        }
    }

    public void InitializePuzzle()
    {
        Transform boxesParent = transform.Find("Boxes");
        boxes = new Box[boxesParent.childCount];

        //Iterate through all the boxes in the puzzle.
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].boxObject = boxesParent.GetChild(i).gameObject;

            // Randomize final dot count for box.
            int random = Random.Range(0, boxes[i].boxObject.transform.childCount + 1);

            boxes[i].activeDots = 0;
            boxes[i].finalDots = random;

            //Iterates through all the dots inside of the box.
            for (int j = boxes[i].boxObject.transform.childCount - 1;  j >= 0 ; j--)
            {
                // Sets the box images based on the dot count.
                Transform dot = boxes[i].boxObject.transform.GetChild(j);
                Image image = dot.GetComponent<Image>();
                image.sprite = (j < boxes[i].finalDots) ? dotFull : dotEmpty;

                StartCoroutine(hide(image));
            } 
        }

        StartCoroutine(showSelection());
    }

    IEnumerator hide(Image image)
    {
        yield return new WaitForSeconds(3);

        image.DOFade(0.0f, 2.0f).OnComplete(()=> {
            Color color = image.color;
            color.a = 1.0f;
            image.color = color;
            image.sprite = dotEmpty;
        });
    }
    IEnumerator showSelection()
    {
        yield return new WaitForSeconds(5);
        waitDone = true;
        SetSelection(0);
    }

    private void SetSelection(int index)
    {
        index = Mathf.Clamp(index, 0, boxes.Length - 1);
        selectedIndex = index;

        for (int i = 0; i < boxes.Length; i++)
        {
            if(index == i)
            {
                boxes[i].boxObject.GetComponent<Image>().color = selectedColor;
                boxes[i].boxObject.transform.DOKill(true);
                boxes[i].boxObject.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.0f), 0.15f, 1, 1.0f);
                boxes[i].isSelected = true;
            }
            else
            {
                boxes[i].boxObject.GetComponent<Image>().color = unselectedColor;
                boxes[i].isSelected = false;
            }

            for (int j = boxes[i].boxObject.transform.childCount - 1; j >= 0; j--)
            {
                // Sets the box images based on the final dot count.
                Transform dotTransform = boxes[i].boxObject.transform.GetChild(j);
                Image image = dotTransform.GetComponent<Image>();
                image.color = (boxes[i].isSelected) ? selectedColor : unselectedColor;

            }
        }
    }

    public void SetDot(int dot)
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            // Rotate all rings that are selected
            if (boxes[i].isSelected) boxes[i].activeDots += dot;

            // Wrap around
            if (boxes[i].activeDots < 0) boxes[i].activeDots = boxes.Length;
            if (boxes[i].activeDots > (boxes.Length )) boxes[i].activeDots = 0;

            for (int j = boxes[i].boxObject.transform.childCount - 1; j >= 0; j--)
            {
                // Sets the box images based on the final dot count.
                Transform dotTransform = boxes[i].boxObject.transform.GetChild(j);
                Image image = dotTransform.GetComponent<Image>();
                image.color = (boxes[i].isSelected) ? selectedColor : unselectedColor;
                image.sprite = (j < boxes[i].activeDots) ? dotFull : dotEmpty;

            }
        }

        SetValidation();
    }

    private void SetValidation()
    {
        bool valid = true;

        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i].activeDots != boxes[i].finalDots) valid = false;
        }

        transform.Find("Check").GetComponent<HologramFX>().showHologram = valid;

        isComplete = valid;
    }
}
