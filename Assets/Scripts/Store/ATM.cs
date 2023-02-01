using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ATM : MonoBehaviour
{
    public TMP_Text rateText;
    MoneyScript rate;

    public TMP_InputField payText;
    int pay;

    // Start is called before the first frame update
    void Start()
    {
        rate = GameObject.Find("Managers").GetComponent<MoneyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        rateText.text = rate.exchangeRate.ToString();
    }

    public void Exchange() //환전
    {
        pay = int.Parse(payText.text);
        if(pay <= MoneyScript.dungeonCoin)
        {
            MoneyScript.dungeonCoin -= pay;
            MoneyScript.moneyGold += (pay * rate.exchangeRate);
        }
        else
        {
            Debug.Log("던전 코인 부족");
            Debug.Log(MoneyScript.dungeonCoin + "까지만 입력 가능합니다");
        }
    }
}
