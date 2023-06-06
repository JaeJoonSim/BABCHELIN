using Microsoft.Cci;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UltimateManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerController playerController
    {
        get
        {
            if (PlayerController == null)
            {
                PlayerController = gameObject.GetComponentInParent<PlayerController>();
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

    private void Start()
    {
        foreach (GameObject go in skills)
        {
            if (go != null)
                go.SetActive(false);
        }
        if (skills[PlayerController.UltIdx] != null)
            skills[PlayerController.UltIdx].SetActive(true);
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

    }

    public void UltimateShot()
    {
        Debug.Log("±Ã±Ø±â");
        gameObject.SetActive(false);
    }
    public void UltimateStart()
    {
        foreach (GameObject go in skills)
        {
            if (go != null)
                go.SetActive(false);
        }
    }


}