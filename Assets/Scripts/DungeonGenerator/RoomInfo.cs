using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RoomInfo
{
    //�� �̸�
    public string roomID;
    //�� ������Ʈ
    public GameObject roomObj;
    // �뿡 �Ҽӵ� ������Ʈ info
    public RoomChild child;
    // ���� ���� ��ġ
    public Vector3Int centerPos;
    // �����;
    //public bool validRoom;
    //�� �湮 ����
    public bool isVisited;
}
