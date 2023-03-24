using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public LayerMask mask;
    public int LastPosX, LastPosZ;
    public float LastPosY;
    private Vector3 mousepos;

    private GameObject plain;
    private Renderer rend;
    public Material matGrid, matDefault;
    private bool isdraging = false;
    private bool placementAble = true;
    private Vector3 previousPoint;

    void Start()
    {
        plain = GameObject.Find("Ground");
        rend = plain.GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        isdraging = true;
        rend.material = matGrid;
    }

    private void OnMouseUp()
    {
        isdraging = false;
        rend.material = matDefault;
    }

    private void OnMouseDrag()
    {
        if (isdraging && placementAble == true)
        {
            mousepos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousepos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                {
                    int PosX = (int)Mathf.Round(hit.point.x);
                    int PosZ = (int)Mathf.Round(hit.point.z);
                    if (PosX != LastPosX || PosZ != LastPosZ)
                    {
                        LastPosX = PosX;
                        LastPosZ = PosZ;
                        transform.position = new Vector3(PosX, LastPosY, PosZ);
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Placement Object"))
        {
            if (gameObject.transform.position == collision.gameObject.transform.position)
            {
                Debug.Log("==");
                placementAble = false;
                //gameObject.transform.position = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
            }
            else
            {
                placementAble = true;
            }
        }
    }
}
