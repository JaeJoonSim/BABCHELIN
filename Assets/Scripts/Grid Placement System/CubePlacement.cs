using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacement : MonoBehaviour
{
    public GameObject ObjToPlace;
    public LayerMask mask;
    public int LastPosX, LastPosZ;
    public float LastPosY;
    public Vector3 mousepos;

    private Renderer rend;
    public Material matGrid,matDefault;

    void Start()
    {
        rend = GameObject.Find("Ground").GetComponent<Renderer>();
        rend.material = matGrid;
    }

    
    void Update()
    {
        mousepos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousepos);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            int PosX = (int)Mathf.Round(hit.point.x);
            int PosZ = (int)Mathf.Round(hit.point.z);

            if(PosX != LastPosX || PosZ != LastPosZ)
            {
                LastPosX = PosX;
                LastPosZ = PosZ;
                transform.position = new Vector3(PosX, LastPosY, PosZ);
                rend.material = matGrid;
            }

            if(Input.GetMouseButtonDown(0))
            {
                Instantiate(ObjToPlace, transform.position, Quaternion.identity);
                Destroy(gameObject);
                rend.material = matDefault;
            }
        }
    }
}
