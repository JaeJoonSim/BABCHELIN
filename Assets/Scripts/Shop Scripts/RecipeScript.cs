using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeScript : MonoBehaviour
{
    [Tooltip("아이템")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    public List<int[]> RecipeArray = new List<int[]>();            //모든 레시피 리스트
    public List<int> FoodArray;
    List<int[]> AbleRecipeNumArray = new List<int[]>();     //가능한 메뉴 띄우기 리스트
    List<int[]> AMenuRecipeArray = new List<int[]>();       //판매할 메뉴 리스트

    // Start is called before the first frame update
    void Start()
    {
        // 리스트에 배열 추가
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
        RecipeArray.Add(new int[] { 6, 4, 5, 6 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 4, 5, 6 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 1, 5, 5 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 5, 6, -1 });      //샌드위치
    }

    void FoodSetting()
    {
        FoodArray.Add(1);   //샌드위치 ID
        FoodArray.Add(2);   //샌드위치 ID;
    }
}
