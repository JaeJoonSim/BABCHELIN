using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sest : MonoBehaviour
{
    private static sest instance;
    public int asd;
    // 데이터 관련 변수들 선언

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        asd++;
    }

    // 인스턴스에 접근하기 위한 정적 메서드
    public static sest Instance
    {
        get { return instance; }
    }
}
