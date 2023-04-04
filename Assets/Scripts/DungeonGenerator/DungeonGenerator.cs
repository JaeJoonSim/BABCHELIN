using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private int distanceBetween;
    private ArrGenerator aGenerator;


    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera vcam;

    void Start()
    {
        aGenerator = GetComponent<ArrGenerator>();
        aGenerator.CreatedRoom();
        GameObject roomObj;
        for (int i = 0; i < DungeonManeger.Instance.ValidRoomList.Count; i++)
        {
            Vector3 pos = new Vector3((DungeonManeger.Instance.ValidRoomList[i].centerPos.z - DungeonManeger.Instance.StartRoomPos.z) * 20 + distanceBetween,
                                           0,
                                           -(DungeonManeger.Instance.ValidRoomList[i].centerPos.x - DungeonManeger.Instance.StartRoomPos.x) * 20 + distanceBetween);
            roomObj = Instantiate(DungeonManeger.Instance.ValidRoomList[i].roomObj, pos, Quaternion.identity);
            //Instantiate(DungeonManeger.Instance.ValidRoomList[i].roomObj, pos, Quaternion.identity);
            roomObj.name = DungeonManeger.Instance.ValidRoomList[i].roomID;
            roomObj.transform.parent = transform;
            DungeonManeger.Instance.ValidRoomList[i].roomObj = roomObj;
            DungeonManeger.Instance.ValidRoomList[i].child = roomObj.GetComponent<RoomChild>();
            roomObj.SetActive(false);
        }
        aGenerator.ConnectDoor();
        DungeonManeger.Instance.ValidRoomList[0].roomObj.SetActive(true);


        PlayerSpawn setVcam = DungeonManeger.Instance.ValidRoomList[0].roomObj.GetComponent<PlayerSpawn>();
        setVcam.Vcam = vcam;
        setVcam.SetFollow();

    }
}
