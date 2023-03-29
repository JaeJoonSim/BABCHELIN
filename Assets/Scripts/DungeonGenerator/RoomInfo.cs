using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RoomInfo
{
    //룸 이름
    public string roomID;
    //룸 오브젝트
    public GameObject roomObj;
    // 룸에 소속된 오브젝트 info
    public RoomChild child;
    // 현재 방의 위치
    public Vector3Int centerPos;
    // 방생성;
    //public bool validRoom;
    //방 방문 여부
    public bool isVisited;
}
