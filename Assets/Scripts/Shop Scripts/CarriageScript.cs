using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageScript : MonoBehaviour
{
    public GameObject Manager;
    TimeScript timeScript;

    private void Start()
    {
        timeScript = Manager.GetComponent<TimeScript>();
    }
    public void OpenCarriage()
    {
        if(timeScript.isNight == true)
        {
            Debug.Log("���� ����");
        }
        else
        {
            Debug.Log("�㿡�� ���� ����");
        }
    }
}
