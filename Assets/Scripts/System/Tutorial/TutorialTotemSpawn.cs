using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTotemSpawn : BaseMonoBehaviour
{
    public GameObject totem;

    bool isSpawn;

    private void Start()
    {
        totem.SetActive(false);
    }

    void Update()
    {
        if (DungeonUIManager.Instance.enemyCount <= 1 && !isSpawn)
        {
            totem.SetActive(true);
            

            isSpawn = true;
        }
    }
}
