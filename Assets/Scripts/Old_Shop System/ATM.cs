using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ATM : MonoBehaviour
{
    public TMP_Text rateText;
    MoneyScript rate;
    public TMP_Text[] rateTextList;

    
    public TMP_InputField payText;
    int payBtnTxt = 0;
    int pay;

    // Start is called before the first frame update
    void Start()
    {
        payText.text = 0.ToString();
        rate = GameObject.Find("Managers").GetComponent<MoneyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        rateText.text = rate.exchangeRate.ToString();

        for (int i = 0; i < 5; i++)
        {
            rateTextList[i].text = rate.rateList[i].ToString();
        }

    }

    //public void Exchange() //환전
    //{
    //    pay = int.Parse(payText.text);

    //    if(pay < 500)
    //    {
    //        Debug.Log("최소 500원부터 환전 가능");
    //        return;
    //    }

    //    if(pay <= MoneyScript.dungeonCoin)
    //    {
    //        if(pay % 10 != 0)
    //        {
    //            pay -= pay % 10;
    //        }

    //        MoneyScript.dungeonCoin -= pay;
    //        MoneyScript.moneyGold += (pay * rate.exchangeRate);
    //    }
    //    else
    //    {
    //        Debug.Log("던전 코인 부족");
    //        Debug.Log(MoneyScript.dungeonCoin + "까지만 입력 가능합니다");
    //    }

    //    payText.text = 0.ToString();
    //}

    public void SumPayTen()
    {
        payBtnTxt = int.Parse(payText.text) + 10;
        payText.text = payBtnTxt.ToString();
    }
    public void SumPayHund()
    {
        payBtnTxt = int.Parse(payText.text) + 100;
        payText.text = payBtnTxt.ToString();
    }
    public void SumPayThou()
    {
        payBtnTxt = int.Parse(payText.text) + 1000;
        payText.text = payBtnTxt.ToString();
    }

    public void aband()     //입력 종료시 1의 자리 버림
    {
        payBtnTxt = int.Parse(payText.text);
        payBtnTxt -= payBtnTxt % 10;
        payText.text = payBtnTxt.ToString();
    }
}
