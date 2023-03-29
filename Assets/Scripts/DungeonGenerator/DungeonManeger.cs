using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManeger : Singleton<DungeonManeger>
{
    //��ǥ�� �����¿� üũ�Ҷ� ���
    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),      // up
        new Vector3Int(1, 0, 0),      // down
        new Vector3Int(0, 0, -1),      // left
        new Vector3Int(0, 0, 1)       // right
    };

    // �ּ� �� ����
    [SerializeField]
    private int maxRoomCount = 15;
    public int MaxRoomCont { get { return maxRoomCount; } }

    // ���� �� ����
    [SerializeField]
    private int curRoomCount = 0;
    public int CurRoomCount { get { return curRoomCount; } set { curRoomCount = value; } }

    // �ִ� �Ÿ� ����
    [SerializeField]
    private int maxDistance = 5;
    public int MaxDistance { get { return maxDistance; } }

    //���۹� ��ǥ
    [SerializeField]
    private Vector3Int startRoomPos;
    public Vector3Int StartRoomPos { get { return startRoomPos; } set { startRoomPos = value; } }

    //ĳ������ ���� �� ��ǥ
    [SerializeField]
    private Vector3Int curPcPos;
    public Vector3Int CurPcPos { get { return curPcPos; } set { curPcPos = value; } }

    //���� �� ������ ����Ʈ
    [SerializeField]
    private List<GameObject> roomList = new List<GameObject>();
    public List<GameObject> RoomList { get { return roomList; } }

    //�� ������ ���� ����Ʈ�� �迭
    private List<RoomInfo> validRoomList = new List<RoomInfo>();
    public List<RoomInfo> ValidRoomList { get { return validRoomList; } set { validRoomList = value; } }

    private RoomInfo[,] posArr = new RoomInfo[10, 10];
    public RoomInfo[,] PosArr { get { return posArr; } set { posArr = value; } }

    //�̴ϸ� ������
    //�̴ϸ� ī�޶�
    [SerializeField]
    private Transform minimapCamera;

    //func
    public void Awake()
    {
        //�迭 �ʱ�ȭ
        PosArr = (RoomInfo[,])ResizeArray(PosArr, new int[] { (MaxDistance * 2), (MaxDistance * 2) });
        RealaseRoomPos();
    }

    //���̵��� ȣ�� ( 0 == ��, 1 == ��, 2 == ��, 3 == ��)
    public void MoveToOtherRoom(int direction)
    {
        if (direction > 0 && 3 < direction)
            return;

        posArr[curPcPos.z, curPcPos.x].roomObj.SetActive(false);
        curPcPos += direction4[direction];
        posArr[curPcPos.z, curPcPos.x].roomObj.SetActive(true);

        minimapCamera.position = posArr[curPcPos.z, curPcPos.x].roomObj.transform.position + new Vector3(0, 20, 0);
    }

    //�迭////////////////////////////////////////////////////////////////////////////////
    //�迭 ���� üũ��
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
    //�迭�� �ʱ�ȭ
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
