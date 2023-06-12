using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public CameraFollowTarget _CamFollowTarget;

    public float _UnscaledTime;
    private float scaledTimeElapsed;

    #region TimeValue
    public float CurrentTime => scaledTimeElapsed;
    public static float DeltaTime => Time.deltaTime * 60f;
    public static float UnscaledDeltaTime => Time.unscaledDeltaTime * 60f;
    public static float FixedDeltaTime => Time.fixedDeltaTime * 60f;
    public static float FixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime * 60f;
    #endregion

    public CameraFollowTarget CamFollowTarget
    {
        get
        {
            if(_CamFollowTarget == null || !_CamFollowTarget.gameObject.activeSelf)
            {
                _CamFollowTarget = CameraFollowTarget.Instance;
            }
            return _CamFollowTarget;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void Update()
    {
        _UnscaledTime = Time.unscaledTime;
        scaledTimeElapsed += Time.deltaTime;
    }

    public float TimeSince(float timestamp)
    {
        return scaledTimeElapsed - timestamp;
    }

    public void AddPlayerToCamera()
    {
        if(PlayerAction.Instance != null)
        {
            GetInstance().AddToCamera(PlayerAction.Instance.CameraBone);
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public void AddToCamera(GameObject gameObject, float Weight = 1f)
    {
        if(!(CamFollowTarget == null))
        {
            CamFollowTarget.AddTarget(gameObject, Weight);
        }
    }

    public void HitStop(float SleepDuration = 0.1f)
    {
        if (Time.timeScale == 1f)
        {
            StartCoroutine(HitStopRoutine(SleepDuration));
        }
    }

    private IEnumerator HitStopRoutine(float SleepDuration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(SleepDuration);
        Time.timeScale = 1f;
    }

    public void ChangeScene(string SceneName)
    {
        //�� �׽�Ʈ�� ���� ��� �ٲ���ϴ�._ �ӵ���
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        DungeonUIManager.Instance.enemyCount = 0;
        //UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
