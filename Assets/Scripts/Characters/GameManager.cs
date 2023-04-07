using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public float _UnscaledTime;
    private float scaledTimeElapsed;

    #region TimeValue
    public float CurrentTime => scaledTimeElapsed;
    public static float DeltaTime => Time.deltaTime * 60f;
    public static float UnscaledDeltaTime => Time.unscaledDeltaTime * 60f;
    public static float FixedDeltaTime => Time.fixedDeltaTime * 60f;
    public static float FixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime * 60f;
    #endregion

    private void Update()
    {
        _UnscaledTime = Time.unscaledTime;
        scaledTimeElapsed += Time.deltaTime;
    }

    public float TimeSince(float timestamp)
    {
        return scaledTimeElapsed - timestamp;
    }
}
