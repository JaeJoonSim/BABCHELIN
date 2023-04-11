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
    public int timeMinute;  //�׽�Ʈ�� public ����
    string AMPM;


    public Image fadeImage;
    public float fadeTime = 1.0f; // ���̵��ξƿ� �ð�
    //private bool isFading = false;

    //public Image testimg;
    //float rotationSpeed = -0.5f; // ȸ�� �ӵ�

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
        time += GameManager.DeltaTime / 5; //���� �ð� = ���� �ð� x 12     GameManager �� �ִ� static DeltaTime ���
        if (time >= 60) //�д��� ����
        {
            timeMinute++;
            time = 0;
        }
        if (timeMinute >= 60) //�ô��� ����
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
        // ���̵�ƿ�
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

        // ���̵���
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