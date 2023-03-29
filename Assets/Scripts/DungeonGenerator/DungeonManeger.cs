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

    [SerializeField]
    private List<GameObject> roomList = new List<GameObject>();
    public List<GameObject> RoomList { get { return roomList; } }

    //맵 관리를 위한 리스트와 배열
    private List<RoomInfo> validRoomList = new List<RoomInfo>();
    public List<RoomInfo> ValidRoomList { get { return validRoomList; } set { validRoomList = value; } }

    private RoomInfo[,] posArr = new RoomInfo[10, 10];
    public RoomInfo[,] PosArr { get { return posArr; } set {  posArr = value; } }



    //func

    public void Awake()
    {
        //배열 초기화
        PosArr = (RoomInfo[,])ResizeArray(PosArr, new int[] { (MaxDistance * 2), (MaxDistance * 2) });
        RealaseRoomPos();
    }

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
}
