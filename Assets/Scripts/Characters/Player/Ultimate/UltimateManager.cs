using Microsoft.Cci;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerController playerController
    {
        get
        {
            if (PlayerController == null)
            {
                PlayerController = transform.root.gameObject.GetComponent<PlayerController>();
            }
            return PlayerController;
        }
    }

    public List<GameObject> skills;
    private bool inArea;

    public float curPos;
    public float min;
    public float max;

    public Sprite GummyBearWhite;
    public Sprite GummyBearRed;

    private void OnEnable()
    {
        if (skills[PlayerController.UltIdx] != null)
            skills[PlayerController.UltIdx].SetActive(true);
    }

    private void OnDisable()
    {
        foreach (GameObject go in skills)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    private void Update()
    {
        Get_MouseInput();
    }

    private void Get_MouseInput()
    {
        switch (playerController.UltIdx)
        {
            case 0:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Utils.GetMouseAngle(transform.position)));
                break;
            case 1:
                skills[playerController.UltIdx].transform.position = Utils.GetMousePosition();
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                curPos = Vector3.Distance(transform.position, skills[playerController.UltIdx].transform.position);

                if (min < curPos && curPos < max)
                { 
                        skills[playerController.UltIdx].GetComponent<SpriteRenderer>().sprite = GummyBearWhite;
                }
                else
                {
                        skills[playerController.UltIdx].GetComponent<SpriteRenderer>().sprite = GummyBearRed;
                }
                break;
            default:
                break;
        }


        //Debug.Log(pos);

    }

    //public static float GetAngle(Vector2 start, Vector2 end)
    //{
    //    return Quaternion.FromToRotation(Vector3.left, start - end).eulerAngles.z;
    //}

}