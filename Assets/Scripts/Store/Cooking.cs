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
        // �ʿ��� ������ ID üũ
        if (cook.Items.Items[0].ID == -1 || cook.Items.Items[1].ID == -1 || cook.Items.Items[2].ID == -1)
        {
            Debug.Log("�丮 ��� ����");
        }
        else  if (cook.Items.Items[0].ID == -1)
        {
            Debug.Log("������ ���� ����");
        }
        else if (cook.Items.Items[0].ID == 1 && cook.Items.Items[1].ID == 2)   // �ʿ��� ������ ID üũ
        {
            // �κ��丮�� ������ �߰�
            //inventory.AddItem(itemDatabase.Items[i].CreateItem(), amount);
        }

    }
}
