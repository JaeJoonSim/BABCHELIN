using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrGenerator : MonoBehaviour
{
    //좌표로 상하좌우 체크할때 사용
    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),      // up
        new Vector3Int(1, 0, 0),      // down
        new Vector3Int(0, 0, -1),      // left
        new Vector3Int(0, 0, 1)       // right
    };

    // 최소 방 갯수
    [SerializeField]
    private int minRoomCnt = 15;

    // 현재 방 갯수
    [SerializeField]
    private int currRoomCnt = 0;
    // 최대 거리 제한
    [SerializeField]
    private int maxDistance = 5;

    //시작방 좌표
    [SerializeField]
    private Vector3Int startRoomPos;
    public Vector3Int StartRoomPos { get { return startRoomPos; } }

    [SerializeField]
    private List<GameObject> roomList = new List<GameObject>();

    //맵 관리를 위한 리스트와 배열
    private List<RoomInfo> validRoomList = new List<RoomInfo>();
    public List<RoomInfo> ValidRoomList { get { return validRoomList; } }
    private RoomInfo[,] posArr = new RoomInfo[10, 10];

    public void CreatedRoom()
    {
        //배열 초기화
        posArr = (RoomInfo[,])ResizeArray(posArr, new int[] { (maxDistance * 2), (maxDistance * 2) });
        RealaseRoomPos();

        currRoomCnt = 0;
        //시작방 랜덤 위치(시작 방 주변으로 방생성하기 위해 랜덤)
        int x = Random.Range(0, maxDistance * 2);
        int z = Random.Range(0, maxDistance * 2);

        startRoomPos = new Vector3Int(z, 0, x);
        posArr[startRoomPos.z, startRoomPos.x] = AddSingleRoom(startRoomPos, "Start");
        validRoomList.Add(posArr[startRoomPos.z, startRoomPos.x]);
        currRoomCnt++;

        while (true)
        {
            if (!(minRoomCnt <= currRoomCnt))
            {
                //랜덤 좌표 추출
                int randRoomIdx = Random.Range(0, validRoomList.Count-1);
                MakeRoomArray(validRoomList[randRoomIdx].centerPos);
            }
            else
                break;
        }
    }

    private RoomInfo AddSingleRoom(Vector3Int pos, string name)
    {
        //방 초기화
        RoomInfo single = new RoomInfo();
        single.roomID = name + "(" + pos.z + ", " + pos.x + ")";
        single.centerPos = pos;
        //single.validRoom = true;

        if (roomList.Count > 0)
        {
            switch (name)
            {
                case "Start":
                    single.roomObj = roomList[0];
                    break;
                case "Single":
                    single.roomObj = roomList[1];
                    break;
                default:
                    break;
            }
        }
        
        single.child = single.roomObj.GetComponent<RoomChild>();

        return single;
    }

    //렌덤 맵 배열 생성
    private void MakeRoomArray(Vector3Int start)
    {
        if ((minRoomCnt <= currRoomCnt))
            return;

        //상하좌우 램덤으로 추가
        int directionsRand = Random.Range(0, direction4.Count);
        Vector3Int newRoom = start + direction4[directionsRand];

        if (!PossibleArr(newRoom))
            return;

        if (posArr[newRoom.z, newRoom.x] == null)
        {
            //방이 안이쁘게 생성되서 한방에 2개 이상 인접하지 못하게 제한
            int closeRoom = 0;

            for (int i = 0; i < direction4.Count; i++)
            {
                Vector3Int isCloseRoom = newRoom + direction4[i];
                if (PossibleArr(isCloseRoom))
                    if (posArr[isCloseRoom.z, isCloseRoom.x] != null)
                        closeRoom++;
            }

            if (closeRoom >= 2)
                return;
            ///

            posArr[newRoom.z, newRoom.x] = AddSingleRoom(newRoom, "Single");
            validRoomList.Add(posArr[newRoom.z, newRoom.x]);
            currRoomCnt++;
            //재귀로 처리
            MakeRoomArray(newRoom);
        }
    }

    //생성된 방 문 연결
    public void ConnectDoor()
    {
        for (int roomIdx = 0; roomIdx < validRoomList.Count; roomIdx++)
        {
            Vector3Int curPos = validRoomList[roomIdx].centerPos;
            for (int isCloseRoom = 0; isCloseRoom < direction4.Count; isCloseRoom++)
            {
                Vector3Int ClosePos = curPos + direction4[isCloseRoom];
                if (PossibleArr(ClosePos))
                {
                    if (posArr[ClosePos.z, ClosePos.x] != null)
                    {
                        switch (isCloseRoom)
                        {
                            case 0:
                                validRoomList[roomIdx].child.upDoor.NextRoom = posArr[ClosePos.z, ClosePos.x].child;
                                break;
                            case 1:
                                validRoomList[roomIdx].child.downDoor.NextRoom = posArr[ClosePos.z, ClosePos.x].child;
                                break;
                            case 2:
                                validRoomList[roomIdx].child.leftDoor.NextRoom = posArr[ClosePos.z, ClosePos.x].child;
                                break;
                            case 3:
                                validRoomList[roomIdx].child.rightDoor.NextRoom = posArr[ClosePos.z, ClosePos.x].child;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    //배열 범위 체크용
    private bool PossibleArr(Vector3Int pos)
    {
        if ((0 <= (pos).x && (pos).x < (maxDistance * 2))
            && (0 <= (pos).z && (pos).z < (maxDistance * 2)))
        {
            return true;
        }
        else
            return false;
    }
    //배열을 초기화
    private static System.Array ResizeArray(System.Array arr, int[] newSizes)
    {
        if (newSizes.Length != arr.Rank)
            return null;

        var temp = System.Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
        int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
        System.Array.ConstrainedCopy(arr, 0, temp, 0, length);
        return temp;
    }
    //배열 초기화
    private void RealaseRoomPos()
    {
        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                posArr[j, i] = null;
            }
        }
    }
}
