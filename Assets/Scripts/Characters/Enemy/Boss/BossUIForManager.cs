using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUIForManager : BaseMonoBehaviour
{
    private void OnEnable()
    {
        DungeonUIManager.Instance.BossUI = gameObject;
    }
}
