using System;
using System.Collections;
using UnityEngine;

public class CameraManager : BaseMonoBehaviour
{
    public enum ShakeMode
    {
        EZCamShake = 0,
        Shake = 1,
        Wobble = 2
    }

    private static float shakeAmount;

    private static float ShakeSpeedX;

    private static float ShakeX;

    private static float ShakeSpeedY;

    private static float ShakeY;

    private static float ShakeZ;

    private static float settings_ScreenShakeIntensity = 1f;

    [HideInInspector] public Vector3 shakeOffset;

    public static CameraManager instance;

    [SerializeField]
    private Camera cameraRef;

    public float TestShakeAmount = 0.4f;

    public float Modifier = 0.1f;

    public float DropOff = 5f;

    public ShakeMode MyShakeMode;

    private float Lerp;

    public float EZModifier = 3f;

    public float EZRoughness = 4f;

    public float EZDuration = 0.4f;

    private float prevEZShakeIntensity;

    private float prevEZShakeTimestamp;

    private bool IsShakingForDuration;

    public Camera CameraRef
    {
        get
        {
            if (!cameraRef)
            {
                return Camera.main;
            }
            return cameraRef;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void Start()
    {
        shakeAmount = 0f;
    }

    private void Update()
    {
        DoShake();
    }

    private void TestShake()
    {
        shakeCamera(TestShakeAmount);
    }

    private void DoShake()
    {
        Vector3 shakeOffset = Vector3.zero;
        switch (MyShakeMode)
        {
            case ShakeMode.Wobble:
                ShakeSpeedX += (0f - ShakeX) * 0.3f;
                ShakeX += (ShakeSpeedX *= 0.6f);
                ShakeSpeedY += (0f - ShakeY) * 0.3f;
                ShakeY += (ShakeSpeedY *= 0.6f);
                shakeOffset = new Vector3(ShakeX, ShakeY) * settings_ScreenShakeIntensity * Time.deltaTime;
                break;
            case ShakeMode.Shake:
                if (ShakeX > 0f)
                {
                    ShakeX -= Time.unscaledDeltaTime * DropOff;
                    ShakeY -= Time.unscaledDeltaTime * DropOff;
                    ShakeZ -= Time.unscaledDeltaTime * DropOff;
                    if (ShakeX <= 0f)
                    {
                        ShakeX = (ShakeY = (ShakeZ = 0f));
                    }
                }
                shakeOffset = UnityEngine.Random.insideUnitSphere * ShakeX * Time.deltaTime;
                break;
        }
        SetShakeOffset(shakeOffset);
    }


    public static float getScreenshakeSettings()
    {
        return settings_ScreenShakeIntensity;
    }

    public void shakeCamera1(float intensity, float direction)
    {
        shakeCamera(intensity, direction);
    }

    public static void shakeCamera(float intensity, bool stackShakes = true)
    {
        shakeCamera(intensity, UnityEngine.Random.Range(0, 360), stackShakes);
    }

    public void EZShake(float intensity, bool stackShakes = true)
    {
        if (GameManager.GetInstance() == null)
        {
            return;
        }
        if (prevEZShakeIntensity > 0f && GameManager.GetInstance().TimeSince(prevEZShakeTimestamp) >= EZDuration)
        {
            prevEZShakeIntensity = 0f;
        }
        if (stackShakes || !(intensity <= prevEZShakeIntensity))
        {
            prevEZShakeIntensity = intensity;
            if (GameManager.GetInstance() != null)
            {
                prevEZShakeTimestamp = GameManager.GetInstance().CurrentTime;
            }
            else
            {
                prevEZShakeTimestamp = Time.time;
            }
            if (CameraShaker.Instance == null)
            {
                CameraShaker cameraShaker = base.gameObject.AddComponent<CameraShaker>();
                cameraShaker.DefaultPosInfluence = Vector3.one * 0.35f;
                cameraShaker.DefaultRotInfluence = Vector3.one * 0.35f;
            }
            CameraShaker.Instance.ShakeOnce(intensity * EZModifier * getScreenshakeSettings(), EZRoughness, 0.1f, EZDuration);
        }
    }

    public static void shakeCamera(float intensity, float direction, bool stackShakes = true)
    {
        if (instance != null)
        {
            switch (instance.MyShakeMode)
            {
                case ShakeMode.EZCamShake:
                    instance.EZShake(intensity, stackShakes);
                    break;
                case ShakeMode.Wobble:
                    ShakeSpeedX = intensity * Mathf.Cos(direction * ((float)Math.PI / 180f)) * getScreenshakeSettings();
                    ShakeSpeedY = intensity * Mathf.Sin(direction * ((float)Math.PI / 180f)) * getScreenshakeSettings();
                    break;
                case ShakeMode.Shake:
                    ShakeX = intensity * getScreenshakeSettings();
                    ShakeY = intensity * getScreenshakeSettings();
                    ShakeZ = intensity * getScreenshakeSettings();
                    break;
            }
        }
    }

    public void ShakeCameraForDuration(float intensityMin, float intensityMax, float duration, bool StackShakes = true)
    {
        if (this != null)
        {
            StartCoroutine(DoShakeCameraForDuration(intensityMin, intensityMax, duration, StackShakes));
        }
    }

    public void Stopshake()
    {
        StopAllCoroutines();
        ShakeX = 0f;
        ShakeY = 0f;
        ShakeZ = 0f;
        ShakeSpeedX = 0f;
        ShakeSpeedY = 0f;
    }

    public void SetShakeOffset(Vector3 offset)
    {
        shakeOffset = offset;
    }

    private IEnumerator DoShakeCameraForDuration(float intensityMin, float intensityMax, float duration, bool StackShakes = true)
    {
        while (true)
        {
            float num;
            duration = (num = duration - Time.deltaTime);
            if (num > 0f)
            {
                if (Time.deltaTime > 0f)
                {
                    shakeCamera(UnityEngine.Random.Range(intensityMin, intensityMax), UnityEngine.Random.Range(0, 360), StackShakes);
                }
                yield return null;
                continue;
            }
            break;
        }
    }

    public static void HitStop()
    {
        instance.StartCoroutine("HitstopCoroutine");
    }

    public IEnumerator HitstopCoroutine()
    {
        Time.timeScale = 0f;
        float RealTimeOfTimestopStart = Time.realtimeSinceStartup;
        float lengthOfTimestop = 0.25f;
        while (Time.realtimeSinceStartup < RealTimeOfTimestopStart + lengthOfTimestop)
        {
            yield return null;
        }
        Time.timeScale = 1f;
    }
}
