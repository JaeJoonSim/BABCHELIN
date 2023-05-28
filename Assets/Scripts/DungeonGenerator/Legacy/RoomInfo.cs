using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RoomInfo
{
    //�� �̸�
    public string roomID;
    public string name;
    //�� ������Ʈ
    public GameObject roomObj;
    //�̴ϸ� ������Ʈ
    public GameObject minimapObj;
    // �뿡 �Ҽӵ� ������Ʈ info
    public RoomChild child;
    // ���� ���� ��ġ
    public Vector3Int centerPos;
    // �����;
    public bool validRoom;
    //�� �湮 ����
    public bool isVisited;
}
