using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TriggerActive : BaseMonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject renderTextureObject;
    private BoxCollider2D boxCollider2D;
    public GameObject DialoguePanel;
    public TextMeshProUGUI PrologueEndText;
    public float textDisplayDuration = 2f;
    public string SceneName = "MainMenu";

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            renderTextureObject.SetActive(true);
            boxCollider2D.enabled = false;
            videoPlayer.Play();
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        renderTextureObject.SetActive(false);
        StartCoroutine(DisplayDialogueAndDisable());
    }

    private IEnumerator DisplayDialogueAndDisable()
    {
        DialoguePanel.SetActive(true);
        yield return new WaitUntil(() => !DialoguePanel.activeInHierarchy);
        TutorialManager.Instance.fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(TutorialManager.Instance.FadeOut(TutorialManager.Instance.fadeDuration));

        StartCoroutine(DisplayPrologueEndText());
    }

    private IEnumerator DisplayPrologueEndText()
    {
        PrologueEndText.gameObject.SetActive(true);
        PrologueEndText.color = new Color(PrologueEndText.color.r, PrologueEndText.color.g, PrologueEndText.color.b, 0f);

        yield return StartCoroutine(FadeInText(PrologueEndText, TutorialManager.Instance.fadeDuration));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeOutText(PrologueEndText, TutorialManager.Instance.fadeDuration));

        PrologueEndText.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneName);
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1, 0, currentTime / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
    }
}
