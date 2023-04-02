using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeScript : MonoBehaviour
{
    [Tooltip("아이템")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    public List<int[]> RecipeArray = new List<int[]>();            //레시피 리스트
    public List<int> FoodArray;                                    //완성요리 리스트

    // Start is called before the first frame update
    void Start()
    {
        // 리스트에 배열 추가

        RecipeSetting();
        FoodSetting();
    }


    // Update is called once per frame
    void Update()
    {

    }


    void RecipeSetting()        //레시피
    {
        RecipeArray.Add(new int[] { 6, 4, 5, 6 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 4, 5, 6 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 1, 5, 5 });      //샌드위치
        RecipeArray.Add(new int[] { 1, 5, 6, -1 });      //샌드위치
    }

    void FoodSetting()          //완성요리
    {
        FoodArray.Add(2);   //망치 ID
        FoodArray.Add(3);   //칼 ID;
        FoodArray.Add(2);   //망치 ID
        FoodArray.Add(3);   //칼 ID;
    }
}
