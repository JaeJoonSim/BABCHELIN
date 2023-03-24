using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageScript : MonoBehaviour
{
    public GameObject Manager;
    TimeScript timeScript;

    [Tooltip("�丮�۾���")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    private void Start()
    {

        timeScript = Manager.GetComponent<TimeScript>();
    }
    public void OpenCarriage()
    {
        if(timeScript.isNight == true)
        {
            if (cook.Items.Items[0].Item.ID == -1 || cook.Items.Items[1].Item.ID == -1 || cook.Items.Items[2].Item.ID == -1 || cook.Items.Items[3].Item.ID == -1)
            {
                Debug.Log("�丮 ��� ����");
            }
            else
            {
                Debug.Log("���� ����");
            }
        }
        else
        {
            Debug.Log("�㿡�� ���� ����");
        }
    }
}
