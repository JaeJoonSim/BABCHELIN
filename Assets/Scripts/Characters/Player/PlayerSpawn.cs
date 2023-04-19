using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject PlayerCharacter;

    void Awake()
    {
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player");;
        absorb.Instance.Player = PlayerCharacter.transform;
    }
}