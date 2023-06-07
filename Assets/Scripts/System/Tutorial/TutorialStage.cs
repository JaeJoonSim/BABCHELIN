using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialStage : BaseMonoBehaviour
{
    public List<GameObject> mapPool;

    public GameObject currentMap;

    public SkeletonAnimation anim;
    public Image image;
    public float playerMoveSpeed = 1.0f;

    public Transform readySpoon;
    public Transform spoon;

    public AnimationReferenceAsset appearance;
    public AnimationReferenceAsset launch;

    private new CameraFollowTarget camera;
    private PlayerController player;

    private float PrevPlayerPos;
    private bool canMoving;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        camera = Camera.main.GetComponent<CameraFollowTarget>();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        anim.AnimationState.Event += OnSpineEvent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && canMoving)
        {
            //StartCoroutine(MapTransition());
            anim.AnimationState.SetAnimation(0, launch, loop: false);
            player.transform.position = readySpoon.transform.position;
            player.enabled = false;
            camera.SnappyMovement = true;
        }
    }

    private void OnEnable()
    {
        if (anim != null)
        {
            anim.AnimationState.SetAnimation(0, appearance, loop: false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (anim.AnimationName == "appearance" && anim.AnimationState.GetCurrent(0).IsComplete)
        {
            canMoving = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canMoving = false;
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Launch")
        {
            StartCoroutine(MapTransition());
            canMoving = false;
        }
    }

    private IEnumerator MapTransition()
    {
        player.transform.SetParent(null);
        player.transform.position = spoon.transform.position;
        player.enabled = false;
        camera.SnappyMovement = true;
        camera.enabled = false;

        player.State.CURRENT_STATE = StateMachine.State.Jump;

        bool isFadeOutComplete = false;
        bool isMoveUpComplete = false;

        StartCoroutine(FadeOut(() => isFadeOutComplete = true));
        StartCoroutine(MovePlayerUp(() => isMoveUpComplete = true));

        yield return new WaitUntil(() => isFadeOutComplete && isMoveUpComplete);
        
        anim.gameObject.SetActive(false);
        player.enabled = true;

        if (currentMap != null) Destroy(currentMap);

        if (mapPool[0] != null)
        {
            currentMap = Instantiate(mapPool[0]);
            mapPool.RemoveAt(0);
        }

        player.transform.position = new Vector3(0, 0, PrevPlayerPos);

        bool isFadeInComplete = false;
        bool isMoveDownComplete = false;

        StartCoroutine(FadeIn(() => isFadeInComplete = true));
        StartCoroutine(MovePlayerDown(() => isMoveDownComplete = true));

        camera.enabled = true;


        yield return new WaitUntil(() => isFadeInComplete && isMoveDownComplete);
        camera.SnappyMovement = false;
        player.State.CURRENT_STATE = StateMachine.State.Landing;

        yield return new WaitForSeconds(0.9f);
        player.State.CURRENT_STATE = StateMachine.State.Idle;
        player.GetComponent<PlayerInput>().enabled = true;
        player.GetComponent<PlayerAction>().enabled = true;
    }

    private IEnumerator FadeOut(Action onComplete)
    {
        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.1f;
            yield return new WaitForSeconds(0.1f);
            image.color = new Color(0, 0, 0, fadeCount);
        }

        onComplete?.Invoke();
    }

    private IEnumerator FadeIn(Action onComplete)
    {
        float fadeCount = 1.0f;
        while (fadeCount > 0)
        {
            fadeCount -= 0.1f;
            yield return new WaitForSeconds(0.1f);
            image.color = new Color(0, 0, 0, fadeCount);
        }

        onComplete?.Invoke();
    }

    private IEnumerator MovePlayerUp(Action onComplete)
    {
        while (player.transform.position.z > -10f)
        {
            player.transform.position -= new Vector3(0, -playerMoveSpeed * Time.deltaTime, playerMoveSpeed * Time.deltaTime);
            yield return null;
        }
        PrevPlayerPos = player.transform.position.z;
        onComplete?.Invoke();
    }

    private IEnumerator MovePlayerDown(Action onComplete)
    {
        while (player.transform.position.z < 0f)
        {
            player.transform.position += new Vector3(0, 0, playerMoveSpeed * Time.deltaTime);
            yield return null;
        }

        onComplete?.Invoke();
    }
}