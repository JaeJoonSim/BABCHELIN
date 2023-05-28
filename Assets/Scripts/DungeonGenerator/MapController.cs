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

    private PlayerController player;
    private new CameraFollowTarget camera;
    private float PrevPlayerPos;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        camera = Camera.main.GetComponent<CameraFollowTarget>();
    }

    public void SelectMap(int choice)
    {
        StartCoroutine(MapTransition(choice));
    }

    private IEnumerator MapTransition(int choice)
    {
        player.enabled = false;
        camera.SnappyMovement = true;
        camera.enabled = false;
        
        player.State.CURRENT_STATE = StateMachine.State.Jump;
        
        bool isFadeOutComplete = false;
        bool isMoveUpComplete = false;

        StartCoroutine(FadeOut(() => isFadeOutComplete = true));
        StartCoroutine(MovePlayerUp(() => isMoveUpComplete = true));

        yield return new WaitUntil(() => isFadeOutComplete && isMoveUpComplete);
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
    }

    public void SpawnBossRoom()
    {
        if (currentMap != null) Destroy(currentMap);

        currentMap = Instantiate(bossRoom);
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
            player.transform.position -= new Vector3(0, 0, playerMoveSpeed * Time.deltaTime);
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
