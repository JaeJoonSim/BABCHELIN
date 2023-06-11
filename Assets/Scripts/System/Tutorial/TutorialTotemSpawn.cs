using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTotemSpawn : BaseMonoBehaviour
{
    public GameObject totem;

    private void Start()
    {
        totem.SetActive(false);
    }

    void Update()
    {
        if (DungeonUIManager.Instance.enemyCount <= 1)
        {
            totem.SetActive(true);
        }
        else
            totem.SetActive(false);
    }
}
