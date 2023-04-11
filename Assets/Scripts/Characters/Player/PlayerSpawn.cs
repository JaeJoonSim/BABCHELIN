using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject PlayerCharacter;
    private GameObject playerObj;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera vcam;
    public Cinemachine.CinemachineVirtualCamera Vcam { set { vcam = value; } }
    void Awake()
    {
        playerObj = Instantiate(PlayerCharacter);
        if(vcam != null)
            vcam.Follow = playerObj.transform;
    }
    public void SetFollow() 
    {
        if (vcam != null)
            vcam.Follow = playerObj.transform;
    }
}