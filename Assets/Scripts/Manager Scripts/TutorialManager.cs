using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : BaseMonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Fade In/Out")]
    public Image fadeImage;
    public float fadeDuration = 3f;

    [Header("Dialogue Popup")]
    public GameObject DialogueUI;
    public float delayInSeconds = 2f;

    [Header("Move Tutorial")]
    public GameObject moveTutorial;
    public Button moveTutorialQuitButton;

    [Header("Other")]
    public StateMachine playerState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        moveTutorialQuitButton.onClick.AddListener(() => QuitPanel(moveTutorial));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && moveTutorial.activeInHierarchy)
        {
            QuitPanel(moveTutorial);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void QuitPanel(GameObject panel)
    {
        panel.SetActive(false);
        playerState.CURRENT_STATE = StateMachine.State.Idle;
        Time.timeScale = 1f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInAndEnableGameObject(fadeDuration, DialogueUI, delayInSeconds));
    }

    public IEnumerator FadeInAndEnableGameObject(float fadeInDuration, GameObject target, float delayAfterFadeIn)
    {
        playerState.CURRENT_STATE = StateMachine.State.Pause;
        yield return StartCoroutine(FadeIn(fadeInDuration));
        yield return new WaitForSeconds(delayAfterFadeIn);

        target.SetActive(true);
        StartCoroutine(ManagePlayerState());
        yield return new WaitUntil(() => !target.activeInHierarchy);
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
        moveTutorial.SetActive(true);
    }

    private IEnumerator ManagePlayerState()
    {
        while (true)
        {
            if (fadeImage.color.a != 0f || moveTutorial.activeInHierarchy || DialogueUI.activeInHierarchy)
            {
                playerState.CURRENT_STATE = StateMachine.State.Pause;
                Time.timeScale = 0f;
            }
            else
            {
                break;
            }
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1, 0, currentTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut(float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
