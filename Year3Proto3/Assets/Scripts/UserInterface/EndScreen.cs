using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public string[] startMessage;
    public float startMessageTime;
    private float startMessageTimer;
    private bool showStartMessage = true;
    private TMP_Text startText;

    private bool showingWin;
    private CanvasGroup winScreenPanel;
    private CanvasGroup winScreenText;
    public float winQuitDelay = 8.0f;
    public float winTimer;
    private bool winQuitStarted;

    private void Start()
    {
        transform.Find("StartText").GetComponent<CanvasGroup>().alpha = 1.0f;
        startText = transform.Find("StartText").GetComponent<TMP_Text>();

        if (GlobalData.deathCount < startMessage.Length)
        {
            startText.text = startMessage[GlobalData.deathCount];
        }
        else
        {
            startText.text = startMessage[startMessage.Length - 1];
        }

        startMessageTimer = startMessageTime * (startText.text.Length * 0.01f);

        winScreenPanel = transform.Find("WinScreen").GetComponent<CanvasGroup>();
        winScreenText = transform.Find("WinScreen/WinText").GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (showStartMessage)
        {
            startMessageTimer -= Time.deltaTime;
        }

        if (startMessageTimer <= 0.0f && showStartMessage)
        {
            startText.GetComponent<CanvasGroup>().DOFade(0.0f, 2.0f);
            showStartMessage = false;
        }

        if (showingWin)
        {
            winTimer += Time.deltaTime;

            if (winTimer >= winQuitDelay && !winQuitStarted)
            {
                FindObjectOfType<SceneSwitcher>().SceneSwitch("TitleScreen");
                winQuitStarted = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowWinScreen();
        }
    }

    public void ShowWinScreen()
    {
        if (!showingWin)
        {
            Sequence winSequence = DOTween.Sequence();
            winSequence.Append(winScreenPanel.DOFade(1.0f, 2.0f).SetEase(Ease.InOutSine));
            winSequence.Append(winScreenText.DOFade(1.0f, 1.5f).SetEase(Ease.InOutSine));

            winSequence.Play();

            showingWin = true;
        }
    }
}