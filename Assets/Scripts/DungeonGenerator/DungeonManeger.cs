using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManeger : Singleton<DungeonManeger>
{
    //좌표로 상하좌우 체크할때 사용
    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),      // up
        new Vector3Int(1, 0, 0),      // down
        new Vector3Int(0, 0, -1),      // left
        new Vector3Int(0, 0, 1)       // right
    };

    public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, 1),   new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 0, 1) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 0, -1)   } }, // 상상우
        {  2, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 0, 1)    } }, // 상상좌
        {  3, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-2, 0, 0)                                } }, // 상상
        {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, 1)                                } }, // 상우
        {  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, -1)                               } }, // 상좌
        {  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                          } }, // 상
    };

    public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(1, 0, -1),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 0, -1) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(2, 0, 0),    new Vector3Int(2, 0, -1)    } }, // 하하우
        {  2, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(2, 0, 0),    new Vector3Int(2, 0, 1)     } }, // 하하좌
        {  3, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(2, 0, 0)                                 } }, // 하하
        {  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(1, 0, 1)                                 } }, // 하우
        {  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),  new Vector3Int(1, 0, -1)                                } }, // 하좌
        {  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                           } }, // 하
    };

    public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1),   new Vector3Int(0, 0, -2),  new Vector3Int(-2, 0, -2) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(0, 0, -2),    new Vector3Int(-1, 0, -2)  } }, // 좌좌상
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(0, 0, -2),    new Vector3Int(1, 0, -2)   } }, // 좌좌하
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(0, 0, -2)                                } }, // 좌좌
        {  4, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1)                               } }, // 좌상
        {  5, new List<Vector3Int>      { new Vector3Int(0, 0, -1), new Vector3Int(1, 0, -1)                                } }, // 좌하
        {  6, new List<Vector3Int>      { new Vector3Int(0, 0, -1),                                                         } }, // 좌 .
    };

    public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1),   new Vector3Int(0, 0, 2),      new Vector3Int(2, 0, 2) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 2),    new Vector3Int(-1, 0, 2)     } }, // 우우상
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 2),    new Vector3Int(1, 0, 2)      } }, // 우우하
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 2)                                  } }, // 우우
        {  4, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(-1, 0, 1)                                 } }, // 우상
        {  5, new List<Vector3Int>      { new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1)                                  } }, // 우하
        {  6, new List<Vector3Int>      { new Vector3Int(0, 0, 1),                                                          } }, // 우
    };

    // 최소 방 갯수
    [SerializeField]
    private int maxRoomCount = 15;
    public int MaxRoomCont { get { return maxRoomCount; } }

    // 현재 방 갯수
    [SerializeField]
    private int curRoomCount = 0;
    public int CurRoomCount { get { return curRoomCount; } set { curRoomCount = value; } }

    // 최대 거리 제한
    [SerializeField]
    private int maxDistance = 5;
    public int MaxDistance { get { return maxDistance; } }

    //시작방 좌표
    [SerializeField]
    private Vector3Int startRoomPos;
    public Vector3Int StartRoomPos { get { return startRoomPos; } set { startRoomPos = value; } }

    //캐릭터의 현재 방 좌표
    [SerializeField]
    private Vector3Int curPcPos;
    public Vector3Int CurPcPos { get { return curPcPos; } set { curPcPos = value; } }

    //던전 룸 프리팹 리스트
    [SerializeField]
    private List<GameObject> roomList = new List<GameObject>();
    public List<GameObject> RoomList { get { return roomList; } }

    //맵 관리를 위한 리스트와 배열
    private List<RoomInfo> validRoomList = new List<RoomInfo>();
    public List<RoomInfo> ValidRoomList { get { return validRoomList; } set { validRoomList = value; } }

    private RoomInfo[,] posArr = new RoomInfo[10, 10];
    public RoomInfo[,] PosArr { get { return posArr; } set { posArr = value; } }

    //미니맵 변수들
    //미니맵 카메라
    [SerializeField]
    private Transform minimapCamera;
    [SerializeField]
    private GameObject visitedTile;
    [SerializeField]
    private GameObject BossTile;

    //func
    public void Awake()
    {
        //배열 초기화
        PosArr = (RoomInfo[,])ResizeArray(PosArr, new int[] { (MaxDistance * 2), (MaxDistance * 2) });
        RealaseRoomPos();
       
    }

    //방이동시 호출 ( 0 == 상, 1 == 하, 2 == 좌, 3 == 우)
    public void MoveToOtherRoom(int direction)
    {
        if (direction > -1 && 3 < direction)
            return;

       
        if(direction != -1)
        {
            posArr[curPcPos.z, curPcPos.x].roomObj.SetActive(false);
            curPcPos += direction4[direction];
            posArr[curPcPos.z, curPcPos.x].roomObj.SetActive(true);
        }
       

        minimapCamera.position = posArr[curPcPos.z, curPcPos.x].roomObj.transform.position + new Vector3(0, 0, -20);



        Vector3Int direction4Pos;
        for (int dir = 0; dir < direction4.Count; dir++)
        {
            direction4Pos = curPcPos + direction4[dir];

            if (!PossibleArr(direction4Pos))
                continue;

            if (posArr[direction4Pos.z, direction4Pos.x] == null)
                continue;

            if (!posArr[direction4Pos.z, direction4Pos.x].isVisited && posArr[direction4Pos.z, direction4Pos.x].roomObj != null)
            {
                posArr[direction4Pos.z, direction4Pos.x].isVisited = true;
                switch (posArr[direction4Pos.z, direction4Pos.x].name)
                {
                    case "Boss":
                        Instantiate(BossTile, 
                            posArr[direction4Pos.z, direction4Pos.x].roomObj.transform.position + new Vector3(0, 0, -10), 
                            visitedTile.transform.rotation);
                        break;
                    default:
                        Instantiate(visitedTile, 
                            posArr[direction4Pos.z, direction4Pos.x].roomObj.transform.position + new Vector3(0, 0, -10), 
                            visitedTile.transform.rotation);
                        break;
                }
                
            }
        }
    }

    //배열////////////////////////////////////////////////////////////////////////////////
    //배열 범위 체크용
    public bool PossibleArr(Vector3Int pos)
    {
        if ((0 <= (pos).x && (pos).x < (MaxDistance * 2))
            && (0 <= (pos).z && (pos).z < (MaxDistance * 2)))
        {
            return true;
        }
        else
            return false;
    }
    //배열을 초기화
    public static System.Array ResizeArray(System.Array arr, int[] newSizes)
    {
        if (newSizes.Length != arr.Rank)
            return null;

        var temp = System.Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
        int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
        System.Array.ConstrainedCopy(arr, 0, temp, 0, length);
        return temp;
    }
    public void RealaseRoomPos()
    {
        for (int i = 0; i < (MaxDistance * 2); i++)
        {
            for (int j = 0; j < (MaxDistance * 2); j++)
            {
                PosArr[j, i] = null;
            }
        }
    }
    ///
}
