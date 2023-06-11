using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatingSpeed = 1f;   // �սǵս� ������ �ӵ�
    public float floatingHeight = 1f;  // �սǵս� ������ ����

    private Vector3 startPos;          // ���� ��ġ

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
            // ���Ʒ��� �սǵս� �����̱�
            Vector3 newPosition = (transform.parent.position + startPos) + Vector3.forward * Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
            transform.position = newPosition;
        }
    }
}
