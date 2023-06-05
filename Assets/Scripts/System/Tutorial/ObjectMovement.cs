using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ObjectMovement : MonoBehaviour
{
    public float targetZ;           // 이동할 목표 z 값
    public float moveDuration;      // 이동에 걸리는 시간
    public float fadeDuration;      // Fade In/Out에 걸리는 시간
    public float fadeInDelay;       // Fade In 시작까지의 대기 시간
    public float fadeOutDelay;      // Fade Out 시작까지의 대기 시간

    [Header("Panel UI")]
    public Transform tutorialTopLetter;
    public Transform tutorialBottomLetter;
    public Image logoImage;         // TutorialGameLogo의 Image 컴포넌트
    public float letterMoveDuration;
    public GameObject dialogue;

    private bool isMoving;          // 이동 중인지 여부

    public Action OnFadeOutComplete;
    public Action OnCameraChangeRotationComplete;

    private Camera mainCam;
    private CameraFollowTarget cam;
    private GameObject player;
    private PlayerInput playerInput;

    private PlayerAction playerAction;

    private void Start()
    {
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraFollowTarget>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player.GetComponent<PlayerInput>();
        playerAction = player.GetComponent<PlayerAction>();

        OnFadeOutComplete += FadeOutCompleted;
        OnCameraChangeRotationComplete += CameraChangeRotationCompleted;
    }


    private void Update()
    {
        if (isMoving)
        {
            cam.SnappyMovement = true;

            float distanceSpeed = (40f - cam.targetDistance) / moveDuration;
            float maxDistanceDelta = distanceSpeed * Time.deltaTime;
            cam.targetDistance = Mathf.MoveTowards(cam.targetDistance, 40f, maxDistanceDelta);

            float moveSpeed = (targetZ) / moveDuration;
            float currentZ = transform.position.z + moveSpeed * Time.deltaTime;

            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);

            if (Mathf.Abs(transform.position.z - targetZ) <= 1)
            {
                isMoving = false;
                playerInput.enabled = true;
                playerAction.enabled = true;
                StartCoroutine(ChangeCameraRotation());
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isMoving)
        {
            StartCoroutine(FadeIn());
            GetComponent<Collider2D>().enabled = false;
            isMoving = true;
            playerInput.enabled = false;
            playerAction.enabled = false;
        }
    }

    private IEnumerator FadeIn()
    {
        StartCoroutine(MoveTutorialLetter(tutorialTopLetter, true));
        StartCoroutine(MoveTutorialLetter(tutorialBottomLetter, false));

        yield return new WaitForSeconds(fadeInDelay);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            logoImage.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            logoImage.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }
        StartCoroutine(MoveTutorialLetter(tutorialTopLetter, false));
        StartCoroutine(MoveTutorialLetter(tutorialBottomLetter, true));

        OnFadeOutComplete?.Invoke();
    }

    private void FadeOutCompleted()
    {
        // TODO...
    }

    private IEnumerator MoveTutorialLetter(Transform letter, bool b)
    {
        Vector3 startPosition = letter.position;
        Vector3 newLetterPosition;
        if (b)
            newLetterPosition = new Vector3(startPosition.x, startPosition.y - 200f, startPosition.z);
        else
            newLetterPosition = new Vector3(startPosition.x, startPosition.y + 200f, startPosition.z);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / letterMoveDuration;
            letter.position = Vector3.Lerp(startPosition, newLetterPosition, t);
            yield return null;
        }
    }

    private IEnumerator ChangeCameraRotation()
    {
        Quaternion startRotation = mainCam.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(-45f, mainCam.transform.eulerAngles.y, mainCam.transform.eulerAngles.z);

        
        cam.targetDistance = 17f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            mainCam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        cam.SnappyMovement = false;
        OnCameraChangeRotationComplete?.Invoke();
    }

    private void CameraChangeRotationCompleted()
    {
        //TODO...
        dialogue.SetActive(true);
    }
}