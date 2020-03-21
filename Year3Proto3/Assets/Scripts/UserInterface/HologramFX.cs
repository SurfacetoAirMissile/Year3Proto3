using DG.Tweening;
using UnityEngine;

public class HologramFX : MonoBehaviour
{
    public bool showHologram = false;
    private bool showing;

    private void Start()
    {
        transform.localScale = new Vector3(1.5f, 0.0f, 1.0f);
    }

    private void Update()
    {
        if (showHologram && !showing)
        {
            Show();
            showing = true;
        }

        if (!showHologram && showing)
        {
            Hide();
            showing = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //showHologram = !showHologram;
        }
    }

    private void Show()
    {
        transform.DOKill(true);
        transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.OutBack);
    }

    private void Hide()
    {
        transform.DOKill(true);
        transform.DOScale(new Vector3(1.5f, 0.0f, 1.0f), 0.15f).SetEase(Ease.OutBack);
    }
}