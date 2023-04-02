using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriageScript : MonoBehaviour
{
    public GameObject Manager;

    public Button OpenBtn;
    ColorBlock OpenBtnColor;
    TimeScript timeScript;
    RecipeScript recipeScript;


    [Tooltip("아이템")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    [Tooltip("요리작업대")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    public GameObject RecipeUI;

    [HideInInspector]
    public List<int> ingreIDArray;                                  //테이블에 올린 재료들의 ID
    public List<int> FoodNumArray = new List<int>();                //완성요리 번호 저장 리스트
    public List<int[]> AbleRecipeNumArray = new List<int[]>();      //올린 재료로 요리 가능한 레시피 리스트

    public GameObject RecipePrefab;
    public GameObject RecipeContent;
    GameObject RecipePrefabObj;

    bool isReady = false;

    public GameObject CookingUI;

    private void Start()
    {
        ingreIDArray = new List<int> { -1, -1, -1, -1 };
        OpenBtnColor = OpenBtn.colors;
        timeScript = Manager.GetComponent<TimeScript>();
        recipeScript = Manager.GetComponent<RecipeScript>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isReady == false)
        {
            ReadyToOpen();
        }
    }

    public void ReadyToOpen()
    {
        if (timeScript.isNight == true)
        {
            if (cook.Items.Items[0].Item.ID == -1)  //메인 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
                isReady = false;
            }
            else if (Cook.Items.Items[1].Item.ID == -1 || Cook.Items.Items[2].Item.ID == -1 || Cook.Items.Items[3].Item.ID == -1)   //서브 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
                isReady = false;
            }
            else
            {
                for (int a = 0; a < 4; a++)
                {
                    ingreIDArray[a] = Cook.Items.Items[a].Item.ID;
                }
                RecipeUIActive();
                RecipeUI.SetActive(true);
                OpenBtnColor.normalColor = new Color(255f, 255f, 255f, 255f);
                OpenBtn.interactable = true;
                isReady = true;
            }
        }
        else
        {
            OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
            OpenBtn.interactable = false;
            isReady = false;
        }
    }

    void RecipeUIActive()
    {
        int recipeNum = 0;

        foreach (int[] recipe in recipeScript.RecipeArray)
        {
            bool isAbleRecipe = true;
            foreach (int num in recipe)
            {
                if (!ingreIDArray.Contains(num))
                {
                    isAbleRecipe = false;
                    break;
                }
            }
            if (isAbleRecipe)
            {
                AbleRecipeNumArray.Add(recipe);
                FoodNumArray.Add(recipeNum);
            }

            recipeNum++;
        }

        recipeNum = 0;
        foreach (int[] ableRecipe in AbleRecipeNumArray)
        {
            RecipePrefabObj = Instantiate(RecipePrefab);
            RecipePrefabObj.transform.SetParent(RecipeContent.transform);

            RecipePrefabObj.transform.GetChild(4).GetComponent<Image>().sprite = items.Items[FoodNumArray[recipeNum] + 1].UiDisplay;     //요리된 음식 아이템 데이터베이스의  recipeNum+1번째 아이템의 이미지 띄우기       //*미완성  완성요리 데이터베이스 작업 필요.  items -> FoodItem 식으로 바꿀 예정
            for (int a = 0; a < items.Items.Length; a++)    //재료 아이템 데이터베이스 ID 비교 후 레시피에 재료 이미지 띄우기
            {
                for (int b = 0; b < 4; b++)     
                {
                    if (ableRecipe[b] == items.Items[a].Data.ID)
                    {
                        RecipePrefab.transform.GetChild(b).GetComponent<Image>().sprite = items.Items[a].UiDisplay;
                    }
                }
            }

            recipeNum++;
        }
    }

    public void InputOpenButton()
    {
        this.gameObject.SetActive(false);
        CookingUI.SetActive(true);
    }
}