using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoneyScript : MonoBehaviour
{
    public static int moneyGold;    //���
    public static int carriageCoin; //���� ����
    public int exchangeRate;               //ȯ��
    public Image arrow;
    public int[] rateList;               //ȯ�� ��ȭ �����
    public TMP_Text goldText;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        moneyGold = 1000;   //�׽�Ʈ�� �⺻ ��ȭ ����
        carriageCoin = 100;
        exchangeRate = 1050;

        InvokeRepeating("ExchangeRateFluctuations", 5f, 5f);    //�׽�Ʈ�� 5�ʸ��� ȯ�� ����
    }

    // Update is called once per frame
    void Update()
    {
        if(moneyGold >= 999999999)
        {
            moneyGold = 999999999;
        }

        goldText.text = moneyGold.ToString();
    }

    void ExchangeRateFluctuations() //ȯ�� ���� �Լ�
    {
        //int per = 100;

        //if (GameObject.Find("Managers").GetComponent<TimeScript>().timeMinute % 5 == 0)
        //{
        //    per = Random.Range(0, 100);
        //}

        int per = Random.Range(0, 100); //ȯ�� ��ȭ Ȯ��

        rateList[4] = rateList[3];
        rateList[3] = rateList[2];
        rateList[2] = rateList[1];
        rateList[1] = rateList[0];
        rateList[0] = exchangeRate;

        if (0 <= per && per < 5)
        {
            exchangeRate = Random.Range(800, 900);
            Debug.Log(exchangeRate);
        }
        else if(5 <= per && per < 15)
        {
            exchangeRate = Random.Range(900, 1000);
            Debug.Log(exchangeRate);
        }
        else if(15 <= per && per < 50)
        {
            exchangeRate = Random.Range(1000, 1100);
            Debug.Log(exchangeRate);
        }
        else if(50 <= per && per < 60)
        {
            exchangeRate = Random.Range(1100, 1200);
            Debug.Log(exchangeRate);
        }
        else if(60 <= per && per < 65)
        {
            exchangeRate = Random.Range(1200, 1300);
            Debug.Log(exchangeRate);
        }
        else
        {

        }

        if(exchangeRate > rateList[0])
        {
            arrow.color = Color.green;
        }
        else if (exchangeRate < rateList[0])
        {
            arrow.color = Color.red;
        }
        else
        {
            arrow.color = Color.gray;
        }
    }
}