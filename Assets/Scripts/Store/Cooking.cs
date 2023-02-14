using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    TimeScript timeState;

    [Tooltip("완성품 인벤토리")]
    [SerializeField]
    private Inventory iceBox;
    public Inventory IceBox { get { return iceBox; } }

    [Tooltip("요리작업대")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }


    [Tooltip("아이템 데이터 베이스")]
    [SerializeField]
    private ItemDatabase itemDatabase;
    public ItemDatabase ItemDatabase { get { return itemDatabase; } }

    public GameObject CookingUI;


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
        bool isReady = false;
        bool isCook = false;
        // 필요한 아이템 ID 체크
        if (cook.Items.Items[0].Item.ID == -1 || cook.Items.Items[1].Item.ID == -1 || cook.Items.Items[2].Item.ID == -1)
        {
            Debug.Log("요리 재료 부족");
        }
        else if (cook.Items.Items[3].Item.ID == -1)
        {
            Debug.Log("촉진제 부족");
        }
        else
        {
            isReady = true;
        }

        //if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce" && cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")   // 필요한 아이템 ID 체크
        //{
        //    isCook = true;
        ////}

        if (isReady == true)
        {
            if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce")   // 필요한 아이템 ID 체크
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("고기 샌드위치 완성");
                    isCook = true;
                }
                else if(cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("고기 샌드위치 완성");
                    isCook = true;
                }
            }
            else if(cook.Items.Items[0].Item.Name == "Inventory.Meat")  
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("야채 샌드위치 완성");
                    isCook = true;
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("야채 샌드위치 완성");
                    isCook = true;
                }
            }
            else if (cook.Items.Items[0].Item.Name == "Inventory.Sandwich")
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("누드 샌드위치 완성");
                    isCook = true;
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("누드 샌드위치 완성");
                    isCook = true;
                }
            }
        }

        if (isCook == true)
        {
            // 인벤토리에 아이템 추가
            //iceBox.AddItem(itemDatabase.Items[0].CreateItem(), 1);
            int a = cook.Items.Items[0].Item.PRICE + cook.Items.Items[1].Item.PRICE + cook.Items.Items[2].Item.PRICE + cook.Items.Items[3].Item.PRICE;  //가격 측정
            
            for(int i = 0; i < 4; i++)
            {
                cook.Items.Items[i].RemoveItem();
            }

            Debug.Log("음식 가격 = " + a);
            CookingUI.SetActive(true);
        }
    }


}
