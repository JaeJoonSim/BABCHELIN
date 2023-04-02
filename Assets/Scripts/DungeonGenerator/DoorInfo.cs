using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfo : MonoBehaviour
{
    public enum roomType {up, down, left, right}

    [SerializeField]
    private RoomChild curRoom;

    [SerializeField]
    private RoomChild nextRoom;
    public RoomChild NextRoom { get { return nextRoom; } set { nextRoom = value; } }

    [SerializeField]
    private roomType type;
    public roomType Type { get { return type; } set { type = value; } }

    private void Update()
    {
        if (NextRoom == null) { gameObject.SetActive(false); }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (Type)
            {
                case roomType.up:
                    DungeonManeger.Instance.MoveToOtherRoom(0);
                    other.transform.root.transform.position = NextRoom.downDoor.transform.position + new Vector3(0,-0.5f, 2);
                    break;
                case roomType.down:
                    DungeonManeger.Instance.MoveToOtherRoom(1);
                    other.transform.root.transform.position = NextRoom.upDoor.transform.position + new Vector3(0, -0.5f, -2);
                    break;
                case roomType.left:
                    DungeonManeger.Instance.MoveToOtherRoom(2);
                    other.transform.root.transform.position = NextRoom.rightDoor.transform.position + new Vector3(-2, -0.5f, 0);
                    break;
                case roomType.right:
                    DungeonManeger.Instance.MoveToOtherRoom(3);
                    other.transform.root.transform.position = NextRoom.leftDoor.transform.position + new Vector3(2, -0.5f, 0);
                    break;
                default:
                    break;
            }          
        }
    }
}
