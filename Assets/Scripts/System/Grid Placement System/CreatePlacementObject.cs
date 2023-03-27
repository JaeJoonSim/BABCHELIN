using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlacementObject : MonoBehaviour
{
    private Renderer rend;
    public Material matGrid;

    void Start()
    {
        rend = GameObject.Find("Ground").GetComponent<Renderer>();
    }

    public void CreateGridMapGameObject(GameObject obj)
    {
        rend.material = matGrid;
        Instantiate(obj, transform.position, Quaternion.identity);
    }
}
