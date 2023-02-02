using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CookingStart()
    {
        // 필요한 아이템 ID 체크
        if (cook.Items.Items[0].ID == -1 || cook.Items.Items[1].ID == -1 || cook.Items.Items[2].ID == -1)
        {
            Debug.Log("요리 재료 부족");
        }
        else  if (cook.Items.Items[0].ID == -1)
        {
            Debug.Log("촉진제 부족 부족");
        }
        else if (cook.Items.Items[0].ID == 1 && cook.Items.Items[1].ID == 2)   // 필요한 아이템 ID 체크
        {
            // 인벤토리에 아이템 추가
            //inventory.AddItem(itemDatabase.Items[i].CreateItem(), amount);
        }

    }
}
