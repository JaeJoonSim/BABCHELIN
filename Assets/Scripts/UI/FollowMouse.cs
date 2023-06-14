using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowMouse : MonoBehaviour
{
    public Vector3 offset = new Vector3(10, 10, 10); // 마우스와의 거리 오프셋

    void Update()
    {
        transform.position = Input.mousePosition + offset;
    }
}
