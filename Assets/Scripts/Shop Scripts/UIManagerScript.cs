using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerScript : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public static bool OnUI;

    // Start is called before the first frame update
    void Start()
    {
        OnUI = false;
    }

    private void Update()
    {
        //PlayerInteraction();
    }

    void PlayerInteraction()
    {
        if(OnUI)
        {
            //PlayerPrefab.GetComponent<PlayerMovement>().enabled = false;
        }
        else
        {
            //PlayerPrefab.GetComponent<PlayerMovement>().enabled = true;
        }
    }

}
