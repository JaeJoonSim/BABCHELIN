using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowMouse : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 10); // ���콺���� �Ÿ� ������

    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
