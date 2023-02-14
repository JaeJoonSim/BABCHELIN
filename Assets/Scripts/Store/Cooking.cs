using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    TimeScript timeState;

    [Tooltip("�ϼ�ǰ �κ��丮")]
    [SerializeField]
    private Inventory iceBox;
    public Inventory IceBox { get { return iceBox; } }

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
        bool isCook = false;
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
            if (cook.Items.Items[0].Item.Name == "Inventory.Lettuce")   // �ʿ��� ������ ID üũ
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("��� ������ġ �ϼ�");
                    isCook = true;
                }
                else if(cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("��� ������ġ �ϼ�");
                    isCook = true;
                }
            }
            else if(cook.Items.Items[0].Item.Name == "Inventory.Meat")  
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Sandwich" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("��ä ������ġ �ϼ�");
                    isCook = true;
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Sandwich" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("��ä ������ġ �ϼ�");
                    isCook = true;
                }
            }
            else if (cook.Items.Items[0].Item.Name == "Inventory.Sandwich")
            {
                if (cook.Items.Items[1].Item.Name == "Inventory.Meat" && cook.Items.Items[2].Item.Name == "Inventory.Lettuce" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("���� ������ġ �ϼ�");
                    isCook = true;
                }
                else if (cook.Items.Items[1].Item.Name == "Inventory.Lettuce" && cook.Items.Items[2].Item.Name == "Inventory.Meat" && cook.Items.Items[3].Item.Name == "Inventory.Salt")
                {
                    Debug.Log("���� ������ġ �ϼ�");
                    isCook = true;
                }
            }
        }

        if (isCook == true)
        {
            // �κ��丮�� ������ �߰�
            //iceBox.AddItem(itemDatabase.Items[0].CreateItem(), 1);
            int a = cook.Items.Items[0].Item.PRICE + cook.Items.Items[1].Item.PRICE + cook.Items.Items[2].Item.PRICE + cook.Items.Items[3].Item.PRICE;  //���� ����
            
            for(int i = 0; i < 4; i++)
            {
                cook.Items.Items[i].RemoveItem();
            }

            Debug.Log("���� ���� = " + a);
            CookingUI.SetActive(true);
        }
    }


}
