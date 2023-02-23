using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cooking : MonoBehaviour
{
    TimeScript timeState;
    public Image cookImage;
    public Slider perfectionBar;
    public TMP_Text perfectionText;

    [Tooltip("냉장고 인벤토리")]
    [SerializeField]
    private Inventory refrigerator;
    public Inventory Refrigerator { get { return refrigerator; } }

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
        //bool isCook = false;
        int price = 0;          //음식 가격
        int perfection = 40;    //기본 완성도 40
        int proficiency = 0;    //기본 숙련도 0;

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
            if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce")   //메인 재료 체크
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
                else if(cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
            }
            else if(cook.Items.Items[0].Item.Name == "Inventory.Meat")  
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
            }
            else if (cook.Items.Items[0].Item.Name == "Inventory.Sandwich")
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //음식 이미지 띄우기
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // 냉장고에 아이템 추가
                }
            }

            price = cook.Items.Items[0].Item.PRICE + cook.Items.Items[1].Item.PRICE + cook.Items.Items[2].Item.PRICE + cook.Items.Items[3].Item.PRICE;  //가격 측정
            perfection += proficiency;          //완성도 측정


            for (int i = 0; i < 4; i++)
            {
                cook.Items.Items[i].RemoveItem();
            }


            Debug.Log("음식 가격 = " + price);
            Debug.Log("음식 완성도 = " + perfection);
            perfectionBar.value = perfection;
            perfectionText.text = perfection.ToString();
            CookingUI.SetActive(true);
        }

    }


}
