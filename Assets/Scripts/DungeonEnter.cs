using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEnter : MonoBehaviour
{
    [Tooltip("던전 조합 테이블")]
    [SerializeField]
    private Inventory dungeonTable;
    public Inventory DungeonTable { get { return dungeonTable; } }
    
    public void CheckingDungeonTable(GameObject dungeonEnterUI)
    {
        for (int i = 0; i < dungeonTable.Items.Items.Length; i++)
        {
            if (dungeonTable.Items.Items[i].Item.ID == -1)
            {
                return;
            }
            else
            {
                dungeonEnterUI.SetActive(true);
            }
        }
    }
}
