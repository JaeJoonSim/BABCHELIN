using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    private Vector2 pivot = new Vector2(-511, -380); 
    List<GameObject> BuffList = new List<GameObject>();
    //RectTransform
    int asss;
    void Start()
    {
        
    }

    void Update()
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            int ypos = (int)(BuffList.Count / 4);
            BuffList[i].transform.position = pivot;
        }

        Debug.Log(asss / 4);
        asss++;
    }
}
