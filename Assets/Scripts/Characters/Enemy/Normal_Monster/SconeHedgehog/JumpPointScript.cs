using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPointScript : MonoBehaviour
{
    float time;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / 1.0334f;
        transform.GetChild(0).localScale = new Vector3(time, time, time);
    }
}
