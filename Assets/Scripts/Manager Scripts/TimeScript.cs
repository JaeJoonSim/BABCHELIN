using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScript : MonoBehaviour
{

    public TMP_Text inGameTime;
    float time;
    public int timeHour;
    public int timeMinute;  //테스트용 public 선언
    string AMPM;


    public Image fadeImage;
    public float fadeTime = 1.0f; // 페이드인아웃 시간
    //private bool isFading = false;

    //public Image testimg;
    //float rotationSpeed = -0.5f; // 회전 속도

    public enum TIMESTATE { Afternoon, Night };

    [SerializeField] private TIMESTATE timeState;
    public TIMESTATE TimeState { get; set; }
    public bool isNight;

    void Start()
    {
        isNight = false;
        AMPM = "AM";
    }

    // Update is called once per frame
    void Update()
    {
        GameTime();
        TimeStateFucn();
        //RotateImage();
    }

    void TimeStateFucn()
    {
        if (7 <= timeHour && timeHour < 18)
        {
            if (timeState != TIMESTATE.Afternoon)
            {
                StartCoroutine(FadeInOut(0.0f, 1.0f));
            }
            timeState = TIMESTATE.Afternoon;
            isNight = false;
        }
        else
        {
            if (timeState != TIMESTATE.Night)
            {
                StartCoroutine(FadeInOut(0.0f, 1.0f));
            }
            timeState = TIMESTATE.Night;
            isNight = true;
        }
    }

    void GameTime()
    {
        //Debug.Log(GameManager.instance._UnscaledTime);
        time += GameManager.DeltaTime / 5; //게임 시간 = 현실 시간 x 12     GameManager 에 있는 static DeltaTime 사용
        if (time >= 60) //분단위 증가
        {
            timeMinute++;
            time = 0;
        }
        if (timeMinute >= 60) //시단위 증가
        {
            timeHour++;
            timeMinute = 0;
        }

        if (12 <= timeHour && timeHour < 24)
        {
            AMPM = "PM";
        }
        else if (timeHour >= 24)
        {
            AMPM = "AM";
            timeHour = 0;
        }

        inGameTime.text = AMPM + " " + string.Format("{0:D2}", timeHour) + ":" + string.Format("{0:D2}", timeMinute);
    }

    private IEnumerator FadeInOut(float startAlpha, float endAlpha)
    {
        // 페이드아웃
        float elapsedTime = 0.0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;

            yield return null;
        }

        // 페이드인
        elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);

            color.a = Mathf.Lerp(endAlpha, startAlpha, t);
            fadeImage.color = color;

            yield return null;
        }
    }


}