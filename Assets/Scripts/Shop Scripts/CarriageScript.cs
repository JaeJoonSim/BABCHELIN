using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriageScript : MonoBehaviour
{
    [Tooltip("아이템")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    [Tooltip("재료 올리는 곳")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    [Tooltip("재료 올리는 곳")]
    [SerializeField]
    private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }


    //오브젝트 선언
    public GameObject Manager;
    Button OpenBtn;
    ColorBlock OpenBtnColor;
    GameObject RecipeUI;
    GameObject RecipeContent;
    public GameObject RecipePrefab;
    GameObject RecipePrefabObj;
    GameObject CookingUI;


    //스크립트
    TimeScript timeScript;
    RecipeScript recipeScript;


    [HideInInspector]
    public List<int> ingreIDArray;                                  //테이블에 올린 재료들의 ID
    public List<int> FoodNumArray = new List<int>();                //완성요리 번호 저장 리스트
    public List<int[]> AbleRecipeNumArray = new List<int[]>();      //올린 재료로 요리 가능한 레시피 리스트

    bool ingreReady = false;
    bool recipeReady = false;


    private void Start()
    {
        //오브젝트 할당
        OpenBtn = this.transform.GetChild(2).GetComponent<Button>();
        OpenBtnColor = OpenBtn.colors;
        RecipeUI = this.transform.GetChild(3).GetChild(0).gameObject;
        RecipeContent = RecipeUI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        CookingUI = this.transform.parent.GetChild(1).gameObject;


        //스크립트 할당
        timeScript = Manager.GetComponent<TimeScript>();
        recipeScript = Manager.GetComponent<RecipeScript>();
        

        gameObject.SetActive(false);
        ingreIDArray = new List<int> { -1, -1, -1, -1 };
    }

    private void Update()
    {
        ReadyToOpen();

        if (ingreReady == true && recipeReady == false)
        {
            RecipeUISetting();
        }


        IngreReset();
    }

    public void ReadyToOpen()
    {
        if (timeScript.isNight == true) //밤일 때 재료 다 넣을 시 버튼 활성화
        {
            if (cook.Items.Items[0].Item.ID == -1)  //메인 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;

                RecipUIReset();
            }
            else if (Cook.Items.Items[1].Item.ID == -1 || Cook.Items.Items[2].Item.ID == -1 || Cook.Items.Items[3].Item.ID == -1)   //서브 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;

                RecipUIReset();
            }
            else
            {
                for (int a = 0; a < 4; a++)     //테이블에 올린 아이템 ID를  ingreIDArray 리스트에 저장
                {
                    ingreIDArray[a] = Cook.Items.Items[a].Item.ID;
                }

                OpenBtnColor.normalColor = new Color(255f, 255f, 255f, 255f);
                OpenBtn.interactable = true;

                RecipeUI.SetActive(true);
                ingreReady = true;
            }
        }
    }

    void RecipeUISetting()
    {
        int recipeNum = 0;


        if (ingreReady == true && recipeReady == false)
        {
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

            RecipeUI.SetActive(true);

            recipeReady = true;
        }
    }

    void RecipUIReset()         //레시피 UI 초기화 함수
    {
        RecipeUI.SetActive(false);
        ingreReady = false;
        recipeReady = false;
        AbleRecipeNumArray = new List<int[]>();
        for (int a = 0; a < RecipeContent.transform.childCount; a++)
        {
            Destroy(RecipeContent.transform.GetChild(a).gameObject);
        }
    }

    void IngreReset()       //UI 끌 시 재료 다시 inventory로 옮기는 함수
    {
        if (!gameObject.activeSelf)
        {
            for(int a = 0; a < 4; a++)
            {

            }

        }
    }

    public void InputOpenButton()
    {
        this.gameObject.SetActive(false);
        CookingUI.SetActive(true);
    }
}