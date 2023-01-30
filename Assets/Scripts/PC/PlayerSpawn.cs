using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject PlayerCharacter;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera vcam;
    void Start()
    {
        var playerObj = Instantiate(PlayerCharacter);
        vcam.Follow = playerObj.transform;
    }
}
