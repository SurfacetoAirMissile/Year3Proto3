using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject fadePanel;
    public float fadeTime = 0.5f;
    private float fadeInDelay = 0.25f;
    private CanvasGroup canvas;
    private string targetScene = "";
    private string curScene = "";
    private bool isFading = false;
    private bool isSwitching = false;
    private float fadeTimeCur = 0.0f;

    private static Vector3 gravity;
    private bool isDead;
    private GameObject fadePanelAlt;
    private Volume darkFog;

    public AudioClip clickSound;
    public AudioClip toolSound;

    void Awake()
    {
        canvas = fadePanel.GetComponent<CanvasGroup>();
        curScene = SceneManager.GetActiveScene().name;
        GlobalData.curScene = curScene;
        Debug.Log("Current scene: " + curScene);
        //clickSound = Resources.Load("Audio/SFX/sfxUIClick2") as AudioClip;
        //toolSound = Resources.Load("Audio/SFX/sfxUIClick3") as AudioClip;
        fadePanelAlt = fadePanel.transform.GetChild(0).gameObject;
        darkFog = GameObject.Find("DarkFog").GetComponent<Volume>();

        if (!GlobalData.gravitySet) 
        { 
            gravity = Physics.gravity;
            GlobalData.gravitySet = true;
        }
    }

    void Start()
    {
        if (curScene == "TitleScreen")
        {
            Physics.gravity = new Vector3(0, 0, 0);
        }

        if (fadePanel == null)
        {
            fadePanel = GameObject.Find("FadePanel(Clone)");
        }

        fadePanel.SetActive(true);

        Invoke("ExitFade", fadeInDelay);
    }

    void Update()
    {
        fadeTimeCur = Mathf.MoveTowards(fadeTimeCur, 0f, Time.unscaledDeltaTime);
        if (fadeTimeCur != 0)
        {
            isFading = true;
        }
        else
        {
            isFading = false;
        }

        if (isSwitching && !isFading)
        {
            switch (targetScene)
            {
                case "":
                    ExitFade();
                    break;
                case "Quit":
                    Application.Quit();
                    break;
                case "Death":
                    GlobalData.deathCount++;
                    isDead = false;
                    SceneManager.LoadScene(curScene);       // Function to run on death fade completion
                    break;
                default:
                    SceneManager.LoadScene(targetScene);
                    Debug.Log("Switched from " + curScene + " to " + targetScene);
                    break;
            }

            isSwitching = false;
        }

        if (curScene == "TitleScreen")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneSwitch("SamLevelDesign");
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            DeathFade();
        }

        fadePanelAlt.SetActive(isDead);
        darkFog.weight = canvas.alpha;
    }

    public void SceneSwitch(string scene)
    {
        StartFade();
        targetScene = scene;
    }

    public void DeathFade()
    {
        StartFade();
        isDead = true;
        targetScene = "Death";
    }

    private void StartFade()
    {
        if (!isSwitching && !isFading)
        {
            Physics.gravity = gravity;
            canvas.DOFade(1.0f, fadeTime).SetEase(Ease.InOutSine);

            isSwitching = true;
            fadeTimeCur = fadeTime;

            AudioSource source = GetComponent<AudioSource>();

            if (source != null)
            {
                source.clip = clickSound;
                source.Play();
            }

        }
        else
        {
            //Debug.LogError("Cannot perform this SceneSwitch while fading in progress!");
        }
    }

    private void ExitFade()
    {
        isFading = true;
        isSwitching = false;
        fadeTimeCur = fadeTime;
        canvas.DOFade(0.0f, fadeTime).SetEase(Ease.InOutSine);
    }

    public void QuitGame()
    {
        StartFade();
        targetScene = "Quit";
        Debug.Log("Silly human. You know you can't quit the game from the editor!");
    }
}