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
                PlayerController = gameObject.GetComponentInParent<PlayerController>();
            }
            return PlayerController;
        }
    }

    public List<GameObject> skills;
    private bool inArea;
    private bool isStart;
    public float curPos;
    public float min;
    public float max;

    public Sprite GummyBearWhite;
    public Sprite GummyBearRed;

    public UltimateStatus Bbebbero;
    public UltimateStatus GummyBear;

    private void OnEnable()
    {
        if (skills[PlayerController.UltIdx] != null)
            skills[PlayerController.UltIdx].SetActive(true);
        isStart = false;
    }

    private void OnDisable()
    {
        foreach (GameObject go in skills)
        {
            if (go != null)
                go.SetActive(false);
        }
        isStart = false;
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
        if (!isStart)
            Get_MouseInput();
    }

    private void Get_MouseInput()
    {
        switch (playerController.UltIdx)
        {
            case 0:
                skills[playerController.UltIdx].transform.localScale = Vector3.one * Bbebbero.size;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Utils.GetMouseAngle(transform.position)));
                inArea = true;
                break;
            case 1:
                skills[playerController.UltIdx].transform.position = Utils.GetMousePosition();
                skills[playerController.UltIdx].transform.localScale = Vector3.one * GummyBear.size;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                curPos = Vector3.Distance(transform.position, skills[playerController.UltIdx].transform.position);

                if (GummyBear.size < curPos && curPos < max + GummyBear.size)
                {
                    inArea = true;
                    skills[playerController.UltIdx].GetComponent<SpriteRenderer>().sprite = GummyBearWhite;
                }
                else
                {
                    inArea = false;
                    skills[playerController.UltIdx].GetComponent<SpriteRenderer>().sprite = GummyBearRed;
                }
                break;
            default:
                break;
        }

    }

    public void UltimateShot()
    {
        gameObject.SetActive(false);
        GameObject temp;
        switch (playerController.UltIdx)
        {
            case 0:
                temp = Instantiate(Bbebbero.UltimateObj, skills[playerController.UltIdx].transform.position, transform.localRotation);
                temp.GetComponent<UltimateBbebbero>().getStatus(Bbebbero);
                break;
            case 1:
                temp = Instantiate(GummyBear.UltimateObj, skills[playerController.UltIdx].transform.position, transform.localRotation);
                temp.GetComponent<UltimateGummyBear>().getStatus(GummyBear);
                break;  
        }

    }
    public bool UltimateStart()
    {
        foreach (GameObject go in skills)
        {
            if (go != null)
                go.SetActive(false);
        }
        isStart = true;

        return inArea;
    }


}