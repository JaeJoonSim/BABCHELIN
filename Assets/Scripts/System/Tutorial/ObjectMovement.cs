using UnityEngine;
using UnityEngine.UI;

public class ObjectMovement : MonoBehaviour
{
    public float targetZ;           // 이동할 목표 z 값
    public float moveDuration;      // 이동에 걸리는 시간
    public float fadeDuration;      // Fade In/Out에 걸리는 시간
    public float fadeInDelay;       // Fade In 시작까지의 대기 시간
    public float fadeOutDelay;      // Fade Out 시작까지의 대기 시간
    public GameObject tutorialPanel;    // TutorialPanel 오브젝트
    public Image logoImage;         // TutorialGameLogo의 Image 컴포넌트

    private Vector3 initialPosition; // 초기 위치
    private bool isMoving;          // 이동 중인지 여부

    private void Start()
    {
        initialPosition = transform.position;
        isMoving = false;
        tutorialPanel.SetActive(false);
        logoImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isMoving)
        {
            float t = Mathf.Clamp01(Time.deltaTime / moveDuration);
            float currentZ = Mathf.Lerp(transform.position.z, targetZ, t);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);

            if (Mathf.Approximately(transform.position.z, targetZ))
            {
                isMoving = false;
                tutorialPanel.SetActive(true);
                logoImage.gameObject.SetActive(true);
                StartCoroutine(MoveAndFade());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CircleCollider2D playerCollider = other.GetComponent<CircleCollider2D>();
            Collider2D moveTriggerCollider = GetComponent<Collider2D>();

            if (playerCollider != null && moveTriggerCollider != null && playerCollider.IsTouching(moveTriggerCollider))
            {
                Debug.Log("Move");
                isMoving = true;
                tutorialPanel.SetActive(true);
                logoImage.gameObject.SetActive(true);
            }
        }
    }

    private System.Collections.IEnumerator MoveAndFade()
    {
        tutorialPanel.SetActive(true);
        logoImage.gameObject.SetActive(true);

        // Fade In 대기
        yield return new WaitForSeconds(fadeInDelay);

        // 로고 Fade In
        float startTime = Time.time;
        Color startColor = logoImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (Time.time - startTime < fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            logoImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        logoImage.color = endColor;

        // 유지 대기
        yield return new WaitForSeconds(moveDuration - fadeInDelay - fadeDuration - fadeOutDelay);

        // 로고 Fade Out
        startTime = Time.time;
        startColor = logoImage.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (Time.time - startTime < fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            logoImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        logoImage.color = endColor;

        // Fade Out 대기
        yield return new WaitForSeconds(fadeOutDelay);

        tutorialPanel.SetActive(false);
        logoImage.gameObject.SetActive(false);
    }
}









