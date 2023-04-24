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

    [Header("Pop Up")]
    public GameObject popupUI;
    public float delayInSeconds = 2f;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInAndEnableGameObject(fadeDuration, popupUI, delayInSeconds));
    }

    public IEnumerator FadeInAndEnableGameObject(float fadeInDuration, GameObject target, float delayAfterFadeIn)
    {
        yield return StartCoroutine(FadeIn(fadeInDuration));
        yield return new WaitForSeconds(delayAfterFadeIn);
        target.SetActive(true);
    }

    public IEnumerator FadeIn(float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, currentTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
