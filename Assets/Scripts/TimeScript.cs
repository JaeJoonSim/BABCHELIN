using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeScript : MonoBehaviour
{
    public TMP_Text inGameTime;
    float time;
    public int timeHour;
    public int timeMinute;  //�׽�Ʈ�� public ����
    string AMPM;

    public enum TIMESTATE { Morning, Afternoon, Dawn };

    public TIMESTATE timeState { get; set; }

    void Start()
    {
        AMPM = "AM";
    }

    // Update is called once per frame
    void Update()
    {
        GameTime();
        TimeState();
    }

    void TimeState()
    {
        if(7 <= timeHour && timeHour < 11)
        {
            timeState = TIMESTATE.Morning;
        }
        else if (11 <= timeHour && timeHour < 17)
        {
            timeState = TIMESTATE.Afternoon;
        }
        else if(17 <= timeHour || timeHour < 7)
        {
            timeState = TIMESTATE.Dawn;
        }
    }

    void GameTime()
    {
        time += Time.deltaTime * 12; //���� �ð� = ���� �ð� x 12

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
}