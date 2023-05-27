using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 50);
    }
}
