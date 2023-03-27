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
    public bool isdraging = false;
    private Vector3 previousPoint;
    private Vector3 unablePoint;

    void Start()
    {
        plain = GameObject.Find("Ground");
        rend = plain.GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        if(isdraging == false)
            previousPoint = transform.position;
        
        isdraging = true;
        rend.material = matGrid;
    }

    private void OnMouseUp()
    {
        isdraging = false;
        rend.material = matDefault;
        
        if (transform.position == unablePoint)
            transform.position = previousPoint;
    }

    private void OnMouseDrag()
    {
        if (isdraging)
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
            if (gameObject.transform.position == collision.gameObject.transform.position && isdraging == true)
            {
                unablePoint = collision.transform.position;
            }
        }
    }
}
