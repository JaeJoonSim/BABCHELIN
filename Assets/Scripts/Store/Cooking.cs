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

    [Tooltip("����� �κ��丮")]
    [SerializeField]
    private Inventory refrigerator;
    public Inventory Refrigerator { get { return refrigerator; } }

    [Tooltip("�丮�۾���")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }


    [Tooltip("������ ������ ���̽�")]
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
        int price = 0;          //���� ����
        int perfection = 40;    //�⺻ �ϼ��� 40
        int proficiency = 0;    //�⺻ ���õ� 0;

        // �ʿ��� ������ ID üũ
        if (cook.Items.Items[0].Item.ID == -1 || cook.Items.Items[1].Item.ID == -1 || cook.Items.Items[2].Item.ID == -1)
        {
            Debug.Log("�丮 ��� ����");
        }
        else if (cook.Items.Items[3].Item.ID == -1)
        {
            Debug.Log("������ ����");
        }
        else
        {
            isReady = true;
        }

        //if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce" && cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")   // �ʿ��� ������ ID üũ
        //{
        //    isCook = true;
        ////}

        if (isReady == true)
        {
            if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce")   //���� ��� üũ
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
                else if(cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
            }
            else if(cook.Items.Items[0].Item.Name == "Inventory.Meat")  
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
            }
            else if (cook.Items.Items[0].Item.Name == "Inventory.Sandwich")
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    cookImage.sprite = itemDatabase.Items[6].UiDisplay;             //���� �̹��� ����
                    refrigerator.AddItem(itemDatabase.Items[6].CreateItem(), 1);    // ����� ������ �߰�
                }
            }

            price = cook.Items.Items[0].Item.PRICE + cook.Items.Items[1].Item.PRICE + cook.Items.Items[2].Item.PRICE + cook.Items.Items[3].Item.PRICE;  //���� ����
            perfection += proficiency;          //�ϼ��� ����


            for (int i = 0; i < 4; i++)
            {
                cook.Items.Items[i].RemoveItem();
            }


            Debug.Log("���� ���� = " + price);
            Debug.Log("���� �ϼ��� = " + perfection);
            perfectionBar.value = perfection;
            perfectionText.text = perfection.ToString();
            CookingUI.SetActive(true);
        }

    }


}
