using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            curRoom.gameObject.SetActive(false);
            NextRoom.gameObject.SetActive(true);
            switch (Type)
            {
                case roomType.up:
                    other.transform.root.transform.position = NextRoom.downDoor.transform.position + new Vector3Int(0, 0, 2);
                    break;
                case roomType.down:
                    other.transform.root.transform.position = NextRoom.upDoor.transform.position + new Vector3Int(0, 0, -2);
                    break;
                case roomType.left:
                    other.transform.root.transform.position = NextRoom.rightDoor.transform.position + new Vector3Int(-2, 0, 0);
                    break;
                case roomType.right:
                    other.transform.root.transform.position = NextRoom.leftDoor.transform.position + new Vector3Int(2, 0, 0);
                    break;
                default:
                    break;
            }          
        }
    }
}
