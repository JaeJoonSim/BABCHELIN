using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : BaseMonoBehaviour
{
    public List<GameObject> mapPool;
    public GameObject bossRoom;
    public GameObject currentMap;
    public Image image;
    public float playerMoveSpeed = 1.0f;
    public GameObject Radial;
    public SkeletonAnimation anim;
    public bool canMove;
    private bool isLaunch;
    private bool isReady;

    [HideInInspector] public int selectedMapIndex;

    private PlayerController player;
    private new CameraFollowTarget camera;
    private Skunk skunk;
    private float PrevPlayerPos;
    private GameObject PrevCameraPos;

    public Transform readySpoon;
    public Transform spoon;
    public AnimationReferenceAsset apperance;
    public AnimationReferenceAsset Launch;


    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        camera = Camera.main.GetComponent<CameraFollowTarget>();

        Radial.SetActive(false);
        anim.gameObject.SetActive(false);

        anim.AnimationState.Event += OnSpineEvent;
    }

    private IEnumerator MoveDurationCamera()
    {
        yield return new WaitForSeconds(2.5f);

        camera.RemoveTarget(camera.targets[0].gameObject);
        camera.AddTarget(PrevCameraPos, 1f);
        camera.targetDistance = 17f;
        camera.distance = 17f;

        yield return null;
    }

    private void Update()
    {
        if (DungeonUIManager.Instance.enemyCount <= 1 && !canMove)
        {
            anim.gameObject.SetActive(true);
            if (anim.gameObject.activeSelf)
            {
                PrevCameraPos = camera.targets[0].gameObject;
                camera.RemoveTarget(camera.targets[0].gameObject);
                camera.AddTarget(anim.gameObject, 1f);
                camera.targetDistance = 11f;
                camera.distance = 11f;

                StartCoroutine(MoveDurationCamera());

                anim.AnimationState.SetAnimation(0, apperance, loop: false);
            }
            canMove = true;
        }

        if (DungeonUIManager.Instance.enemyCount > 1)
        {
            canMove = false;
            anim.gameObject.SetActive(false);
        }

        if (!canMove)
        {
            anim.gameObject.SetActive(false);
        }

        if(isLaunch)
        {
            StartCoroutine(MapTransition(selectedMapIndex));
            isLaunch = false;
        }

        if (isReady && !isLaunch && anim.AnimationName == Launch.Animation.Name)
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, -0.7f, player.transform.position.z);
            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, Time.deltaTime * 3);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canMove)
        {
            Radial.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Radial.SetActive(false);
        }
    }

    public void SelectMap()
    {
        anim.AnimationState.SetAnimation(0, Launch, loop: false);
        player.transform.position = readySpoon.transform.position;
        player.enabled = false;
        camera.SnappyMovement = true;
        isReady = true;
    }

    public void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Launch")
        {
            Radial.SetActive(false);
            isLaunch = true;
            isReady = false;
        }
    }

    private IEnumerator MapTransition(int choice)
    {
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
        canMove = false;
        anim.gameObject.SetActive(false);
        player.enabled = true;

        if (mapPool.Count <= 3)
        {
            SpawnBossRoom();
        }
        else
        {
            if (currentMap != null) Destroy(currentMap);

            currentMap = Instantiate(mapPool[choice]);
            mapPool.RemoveAt(choice);

            System.Random rng = new System.Random();
            int n = mapPool.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameObject value = mapPool[k];
                mapPool[k] = mapPool[n];
                mapPool[n] = value;
            }
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
        skunk.patternManager.enabled = true;
    }

    public void SpawnBossRoom()
    {
        if (currentMap != null) Destroy(currentMap);

        currentMap = Instantiate(bossRoom);

        skunk = FindObjectOfType<Skunk>();

        skunk.patternManager.enabled = false;
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
