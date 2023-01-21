using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScript : MonoBehaviour
{
    public TMP_Text inGameTime;
    float time;
    int timeHour;
    int timeMinute;
    string AMPM;

    enum State { Morning, Afternoon, Dawn };

    State state;

    void Start()
    {
        AMPM = "AM";
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * 12; //게임 시간 = 현실 시간 x 12

        if(time >= 60) //분단위 증가
        {
            timeMinute++;
            time = 0;
        }
        if(timeMinute >= 60) //시단위 증가
        {
            timeHour++;
            timeMinute = 0;
        }

        if(12 <= timeHour && timeHour < 24)
        {
            AMPM = "PM";
        }
        else if(timeHour >= 24)
        {
            AMPM = "AM";
            timeHour = 0;
        }

        inGameTime.text = AMPM + " " + string.Format("{0:D2}", timeHour) + ":" + string.Format("{0:D2}", timeMinute);
    }

    void TimeState()
    {
        if(7 <= timeHour && timeHour < 11)
        {
            state = State.Morning;
        }
        else if (11 <= timeHour && timeHour < 17)
        {
            state = State.Afternoon;
        }
        else if(17 <= timeHour || timeHour < 7)
        {
            state = State.Dawn;
        }
    }
}