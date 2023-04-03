using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonSelectScript : MonoBehaviour
{
    int DengeonNum;
    public Image DungeonThemeImage;
    public Sprite[] DungeonThemeImageArray;

    public GameObject CheckInUI;

    // Start is called before the first frame update
    void Start()
    {
        DengeonNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(DengeonNum <= 0)
        {
            DengeonNum = 0;
        }
        else if(DengeonNum >= 2)
        {
            DengeonNum = 2;
        }

        DungeonThemeImage.sprite = DungeonThemeImageArray[DengeonNum];
    }

    public void InputDungeonButton()
    {
        //들어가는 던전의 번호 저장 작업 필요
        CheckInUI.SetActive(true);
    }
    
    public void InputLeftBtn()
    {
        DengeonNum--;
    }

    public void InputRightBtn()
    {
        DengeonNum++;
    }

    public void InputYesBtn()
    {
        //들어가는 던전의 번호 저장 작업 필요
        SceneManager.LoadScene("DungeonGenerator");
        UIManagerScript.OnUI = !UIManagerScript.OnUI;
    }

    public void InputNoBtn()
    {
        CheckInUI.SetActive(false);
    }

}
