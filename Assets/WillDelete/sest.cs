using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sest : MonoBehaviour
{
    private static sest instance;
    public int asd;
    // ������ ���� ������ ����

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

    // �ν��Ͻ��� �����ϱ� ���� ���� �޼���
    public static sest Instance
    {
        get { return instance; }
    }
}
