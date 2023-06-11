using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatingSpeed = 1f;   // 둥실둥실 움직임 속도
    public float floatingHeight = 1f;  // 둥실둥실 움직임 높이

    private Vector3 startPos;          // 시작 위치

    public bool ismoving;

    private void Start()
    {
        ismoving = true;
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if (ismoving)
        {
            // 위아래로 둥실둥실 움직이기
            Vector3 newPosition = (transform.parent.position + startPos) + Vector3.forward * Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
            transform.position = newPosition;
        }
    }
}
