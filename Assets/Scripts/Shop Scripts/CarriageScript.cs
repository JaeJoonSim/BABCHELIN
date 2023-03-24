using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageScript : MonoBehaviour
{
    public GameObject Manager;
    TimeScript timeScript;

    [Tooltip("요리작업대")]
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
                Debug.Log("요리 재료 부족");
            }
            else
            {
                Debug.Log("마차 오픈");
            }
        }
        else
        {
            Debug.Log("밤에만 오픈 가능");
        }
    }
}
