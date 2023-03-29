using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeScript : MonoBehaviour
{
    [Tooltip("������")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    public List<int[]> RecipeArray = new List<int[]>();            //��� ������ ����Ʈ
    public List<int> FoodArray;
    List<int[]> AbleRecipeNumArray = new List<int[]>();     //������ �޴� ���� ����Ʈ
    List<int[]> AMenuRecipeArray = new List<int[]>();       //�Ǹ��� �޴� ����Ʈ

    // Start is called before the first frame update
    void Start()
    {
        // ����Ʈ�� �迭 �߰�
        for (int i = 0; i < 20; i++)
        {
            RecipeArray.Add(new int[5]);
        }

        RecipeSetting();
        FoodSetting();
    }


    // Update is called once per frame
    void Update()
    {

    }


    void RecipeSetting()
    {
        RecipeArray.Add(new int[] { 6, 4, 5, 6 });      //������ġ
        RecipeArray.Add(new int[] { 1, 4, 5, 6 });      //������ġ
        RecipeArray.Add(new int[] { 1, 1, 5, 5 });      //������ġ
        RecipeArray.Add(new int[] { 1, 5, 6, -1 });      //������ġ
    }

    void FoodSetting()
    {
        FoodArray.Add(1);   //������ġ ID
        FoodArray.Add(2);   //������ġ ID;
    }
}
