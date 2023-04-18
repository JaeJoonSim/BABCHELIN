using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject PlayerCharacter;

    void Awake()
    {
        PlayerCharacter = Instantiate(PlayerCharacter);

    }
}