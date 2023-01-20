using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularArrangment : MonoBehaviour
{
    [Tooltip("¹ÝÁö¸§")]
    [SerializeField]
    private float radius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        int numOfChild = transform.childCount;

        for(int i = 0; i < numOfChild; i++)
        {
            float angle = i * (Mathf.PI * 2.0f) / numOfChild;

            GameObject child = transform.GetChild(i).gameObject;

            child.transform.position = transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * radius;
        }
    }
}
