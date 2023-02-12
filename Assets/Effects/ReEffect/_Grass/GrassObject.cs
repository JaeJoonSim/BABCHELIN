using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassObject : MonoBehaviour
{
    public Material mat;
    public string Pos = "player_position";
    public Transform player;


    private void LateUpdate()
    {
        if (player != null)
        {
            mat.SetVector(Pos, player.position);
        }
        else
        {
            Debug.LogError("no Player found for grass deformation");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
