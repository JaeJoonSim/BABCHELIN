using UnityEngine;
using UnityEngine.UI;

public class ObjectMovement : MonoBehaviour
{
    public float targetZ;           // �̵��� ��ǥ z ��
    public float moveDuration;      // �̵��� �ɸ��� �ð�
    public float fadeDuration;      // Fade In/Out�� �ɸ��� �ð�
    public float fadeInDelay;       // Fade In ���۱����� ��� �ð�
    public float fadeOutDelay;      // Fade Out ���۱����� ��� �ð�
    public GameObject tutorialPanel;    // TutorialPanel ������Ʈ
    public Image logoImage;         // TutorialGameLogo�� Image ������Ʈ

    private Vector3 initialPosition; // �ʱ� ��ġ
    private bool isMoving;          // �̵� ������ ����

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

        // Fade In ���
        yield return new WaitForSeconds(fadeInDelay);

        // �ΰ� Fade In
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

        // ���� ���
        yield return new WaitForSeconds(moveDuration - fadeInDelay - fadeDuration - fadeOutDelay);

        // �ΰ� Fade Out
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

        // Fade Out ���
        yield return new WaitForSeconds(fadeOutDelay);

        tutorialPanel.SetActive(false);
        logoImage.gameObject.SetActive(false);
    }
}









