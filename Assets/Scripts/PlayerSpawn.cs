using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject PlaterCharacter;

    void Start()
    {
        Instantiate(PlaterCharacter);
    }
}
