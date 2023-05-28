
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrGenerator : MonoBehaviour
{
    public void CreatedRoom()
    {
        int count = 0;

        DungeonManeger.Instance.CurRoomCount = 0;
        //���۹� ���� ��ġ(���� �� �ֺ����� ������ϱ� ���� ����)
        int x = Random.Range(0, DungeonManeger.Instance.MaxDistance * 2);
        int z = Random.Range(0, DungeonManeger.Instance.MaxDistance * 2);

        DungeonManeger.Instance.StartRoomPos = new Vector3Int(z, 0, x);
        DungeonManeger.Instance.CurPcPos = DungeonManeger.Instance.StartRoomPos;
        DungeonManeger.Instance.PosArr[DungeonManeger.Instance.StartRoomPos.z, DungeonManeger.Instance.StartRoomPos.x]
            = AddSingleRoom(DungeonManeger.Instance.StartRoomPos, "Start");

        DungeonManeger.Instance.ValidRoomList.Add(
            DungeonManeger.Instance.PosArr[DungeonManeger.Instance.StartRoomPos.z, DungeonManeger.Instance.StartRoomPos.x]);

        DungeonManeger.Instance.CurRoomCount++;

        while (count < 100)
        {
            count++;
            //Debug.Log(count);
            if (!(DungeonManeger.Instance.MaxRoomCont <= DungeonManeger.Instance.CurRoomCount))
            {
                //���� ��ǥ ����
                int randRoomIdx = Random.Range(0, DungeonManeger.Instance.ValidRoomList.Count - 1);
                MakeRoomArray(DungeonManeger.Instance.ValidRoomList[randRoomIdx].centerPos);
            }
            else
                break;
        }

        ChangeSingleRoom(DungeonManeger.Instance.ValidRoomList[DungeonManeger.Instance.ValidRoomList.Count - 1], "Boss");

    }
    private RoomInfo AddSingleRoom(Vector3Int pos, string name)
    {
        //�� �ʱ�ȭ
        RoomInfo single = new RoomInfo();
        single.roomID = name + "(" + pos.z + ", " + pos.x + ")";
        single.name = name;
        single.centerPos = pos;
        single.validRoom = true;
        single.isVisited = false;
        single.minimapObj = null;
        if (DungeonManeger.Instance.RoomList.Count > 0)
        {
            switch (name)
            {
                case "Null":
                    single.roomObj = null;
                    single.validRoom = false;
                    single.isVisited = true;
                    break;
                case "Start":
                    single.roomObj = DungeonManeger.Instance.RoomList[0];

                    Vector3Int nullRoom = pos + DungeonManeger.Instance.direction4[0];

                    if (DungeonManeger.Instance.PossibleArr(nullRoom))
                        DungeonManeger.Instance.PosArr[nullRoom.z, nullRoom.x] = AddSingleRoom(nullRoom, "Null");

                    break;
                case "Single":
                    single.roomObj = DungeonManeger.Instance.RoomList[1];
                    break;
                default:
                    break;
            }
        }

        if (single.roomObj != null)
        {
            single.child = single.roomObj.GetComponent<RoomChild>();
        }


        return single;
    }

    private void ChangeSingleRoom(RoomInfo value, string name)
    {
        value.name = name;
        switch (name)
        {
            case "Null":
                value.roomObj = null;
                value.validRoom = false;
                value.isVisited = true;
                break;
            case "Single":
                value.roomObj = DungeonManeger.Instance.RoomList[1];
                break;
            case "Boss":
                value.roomObj = DungeonManeger.Instance.RoomList[DungeonManeger.Instance.RoomList.Count - 1];
                break;
            default:
                break;
        }
    }

    //���� �� �迭 ����
    private void MakeRoomArray(Vector3Int start)
    {
        if ((DungeonManeger.Instance.MaxRoomCont <= DungeonManeger.Instance.CurRoomCount))
            return;

        //�����¿� �������� �߰�
        int directionsRand = Random.Range(0, DungeonManeger.Instance.direction4.Count);
        Vector3Int newRoom = start + DungeonManeger.Instance.direction4[directionsRand];


        if (!DungeonManeger.Instance.PossibleArr(newRoom))
            return;


        if (DungeonManeger.Instance.PosArr[newRoom.z, newRoom.x] == null)
        {
            //���� ���̻ڰ� �����Ǽ� �ѹ濡 2�� �̻� �������� ���ϰ� ����
            int closeRoom = 0;

            for (int i = 0; i < DungeonManeger.Instance.direction4.Count; i++)
            {
                Vector3Int isCloseRoom = newRoom + DungeonManeger.Instance.direction4[i];
                if (DungeonManeger.Instance.PossibleArr(isCloseRoom))
                    if (DungeonManeger.Instance.PosArr[isCloseRoom.z, isCloseRoom.x] != null)
                        closeRoom++;
            }

            if (closeRoom >= 2)
                return;
            ///

            DungeonManeger.Instance.PosArr[newRoom.z, newRoom.x] = AddSingleRoom(newRoom, "Single");
            DungeonManeger.Instance.ValidRoomList.Add(DungeonManeger.Instance.PosArr[newRoom.z, newRoom.x]);
            DungeonManeger.Instance.CurRoomCount++;
            //��ͷ� ó��
            MakeRoomArray(newRoom);
        }
    }

    //������ �� �� ����
    public void ConnectDoor()
    {
        for (int roomIdx = 0; roomIdx < DungeonManeger.Instance.ValidRoomList.Count; roomIdx++)
        {
            Vector3Int curPos = DungeonManeger.Instance.ValidRoomList[roomIdx].centerPos;
            for (int isCloseRoom = 0; isCloseRoom < DungeonManeger.Instance.direction4.Count; isCloseRoom++)
            {
                Vector3Int ClosePos = curPos + DungeonManeger.Instance.direction4[isCloseRoom];
                if (DungeonManeger.Instance.PossibleArr(ClosePos))
                {
                    if (DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x] != null)
                    {
                        if (DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x].validRoom)
                        {
                            switch (isCloseRoom)
                            {
                                case 0:
                                    DungeonManeger.Instance.ValidRoomList[roomIdx].child.upDoor.NextRoom
                                        = DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x].child;
                                    break;
                                case 1:
                                    DungeonManeger.Instance.ValidRoomList[roomIdx].child.downDoor.NextRoom
                                        = DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x].child;
                                    break;
                                case 2:
                                    DungeonManeger.Instance.ValidRoomList[roomIdx].child.leftDoor.NextRoom
                                        = DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x].child;
                                    break;
                                case 3:
                                    DungeonManeger.Instance.ValidRoomList[roomIdx].child.rightDoor.NextRoom
                                        = DungeonManeger.Instance.PosArr[ClosePos.z, ClosePos.x].child;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }


}
