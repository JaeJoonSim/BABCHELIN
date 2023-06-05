using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject MonsterUICanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < 0)
        {
            MonsterUICanvas.transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            MonsterUICanvas.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
