using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[SerializeField]
	private int distanceBetween;
    private ArrGenerator aGenerator;

    void Start()
    {
        aGenerator = GetComponent<ArrGenerator>();
        aGenerator.CreatedRoom();
        GameObject roomObj;
        for (int i = 0; i < aGenerator.ValidRoomList.Count; i++)
        {
            Vector3 pos = new Vector3Int((aGenerator.ValidRoomList[i].centerPos.z - aGenerator.StartRoomPos.z) * (10+distanceBetween),
                                           0,
                                           -(aGenerator.ValidRoomList[i].centerPos.x - aGenerator.StartRoomPos.x) * (10+distanceBetween));
            roomObj = Instantiate(aGenerator.ValidRoomList[i].roomObj, pos,Quaternion.identity);
            roomObj.name = aGenerator.ValidRoomList[i].roomID;
            roomObj.transform.parent = transform;
            aGenerator.ValidRoomList[i].roomObj = roomObj;
            aGenerator.ValidRoomList[i].child = roomObj.GetComponent<RoomChild>();
            roomObj.SetActive(false);
        }
        aGenerator.ConnectDoor();
        aGenerator.ValidRoomList[0].roomObj.SetActive(true);
    }
}
