using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : BaseMonoBehaviour
{
    public List<GameObject> mapPool;
    public GameObject bossRoom;
    public GameObject currentMap;

    public void SelectMap(int choice)
    {
        if (mapPool.Count <= 3)
        {
            SpawnBossRoom();
        }
        else
        {
            if (currentMap != null) Destroy(currentMap);

            currentMap = Instantiate(mapPool[choice]);
            mapPool.RemoveAt(choice);

            System.Random rng = new System.Random();
            int n = mapPool.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameObject value = mapPool[k];
                mapPool[k] = mapPool[n];
                mapPool[n] = value;
            }
        }
    }

    public void SpawnBossRoom()
    {
        if (currentMap != null) Destroy(currentMap);

        currentMap = Instantiate(bossRoom);
    }
}