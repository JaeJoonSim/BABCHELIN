using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionSceneChange : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _promt;
    public string InteractionPrompt => _promt;
    public string SceneName;

    [Space]
    public SkeletonAnimation anim;
    public AnimationReferenceAsset launch;
    public Transform readySpoon;
    public Transform spoon;

    #region Unity Events
    [Space]
    public UnityEvent onInteraction;
    public UnityEvent offInteraction;
    #endregion

    private new CameraFollowTarget camera;
    private PlayerController player;
    private bool isReady;

    private void Start()
    {
        camera = Camera.main.GetComponent<CameraFollowTarget>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        OffInteract();
    }

    public bool OnInteract(Interactor interactor)
    {
        onInteraction.Invoke();
        Debug.Log($"{gameObject.name} : OnInteracted with SceneChange");
        StartCoroutine(InteractSpoon());
        return true;
    }

    public void OffInteract()
    {
        
    }

    private IEnumerator InteractSpoon()
    {
        anim.AnimationState.SetAnimation(0, launch, loop: false);
        player.transform.position = readySpoon.transform.position;
        player.enabled = false;
        camera.SnappyMovement = true;
        isReady = true;

        yield return new WaitForSeconds(1.0f);

        player.transform.position = spoon.transform.position;
        player.enabled = false;
        camera.SnappyMovement = true;
        camera.enabled = false;

        StartCoroutine(MovePlayerUp(() =>
        {
            player.enabled = true;
            camera.SnappyMovement = false;
            camera.enabled = true;
            isReady = false;
            LoadScene(SceneName);
        }));
    }

    private IEnumerator MovePlayerUp(Action onComplete)
    {
        while (player.transform.position.z > -10f)
        {
            player.transform.position -= new Vector3(0, -15f * Time.deltaTime, 15f * Time.deltaTime);
            yield return null;
        }
        
        onComplete?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        PlayerPrefs.SetString("SceneToLoad", sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }
}
