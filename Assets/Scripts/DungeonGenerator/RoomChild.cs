using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class RoomChild : MonoBehaviour
{
    public DoorInfo upDoor;
    public DoorInfo downDoor;
    public DoorInfo leftDoor;
    public DoorInfo rightDoor;

    private List<DoorInfo> door = new List<DoorInfo> { };

    public List<GameObject> AbsorbObject = new List<GameObject> { };
    public List<GameObject> DestructObject = new List<GameObject> { };

    private void Awake()
    {
        door.Add(upDoor);
        door.Add(downDoor);
        door.Add(leftDoor);
        door.Add(rightDoor);
    }
}
