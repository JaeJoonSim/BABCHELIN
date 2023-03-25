using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriageScript : MonoBehaviour
{
    public GameObject Manager;
    public Button OpenBtn;
    ColorBlock OpenBtnColor;
    TimeScript timeScript;

    [Tooltip("요리작업대")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    private void Start()
    {
        OpenBtnColor = OpenBtn.colors;
        timeScript = Manager.GetComponent<TimeScript>();
    }

    private void Update()
    {
        OpenButtonActive();
    }
    public void OpenButtonActive()
    {
        if(timeScript.isNight == true)
        {
            if (cook.Items.Items[0].Item.ID == -1)
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
            }
            else if(cook.Items.Items[1].Item.ID == -1 || cook.Items.Items[2].Item.ID == -1 || cook.Items.Items[3].Item.ID == -1)
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
            }
            else
            {
                OpenBtnColor.normalColor = new Color(255f, 255f, 255f, 255f);
                OpenBtn.interactable = true;
            }
        }
        else
        {
            OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
            OpenBtn.interactable = false;
        }
    }
}
