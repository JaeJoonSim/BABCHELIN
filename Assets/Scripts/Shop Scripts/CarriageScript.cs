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
    List<int> ingreIDArr;
    List<int[]> AbleRecipeNumArray = new List<int[]>();

    public GameObject RecipePrefab;
    public GameObject RecipeContent;

    bool isReady = false;

    private void Start()
    {
        ingreIDArr = new List<int> { -1, -1, -1, -1 };
        OpenBtnColor = OpenBtn.colors;
        timeScript = Manager.GetComponent<TimeScript>();
        recipeScript = Manager.GetComponent<RecipeScript>();
    }

    private void Update()
    {
        if(isReady == false)
        {
            ReadyToOpen();
        }
    }

    public void ReadyToOpen()
    {
        if(timeScript.isNight == true)
        {
            if (cook.Items.Items[0].Item.ID == -1)  //메인 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
                isReady = false;
            }
            else if(Cook.Items.Items[1].Item.ID == -1 || Cook.Items.Items[2].Item.ID == -1 || Cook.Items.Items[3].Item.ID == -1)   //서브 재료 없음
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
                isReady = false;
            }
            else
            {
                for(int a = 0; a < 4; a++)
                {
                    ingreIDArr[a] = Cook.Items.Items[a].Item.ID;
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
        Debug.Log("생성 시작");
        //List<int> A = new List<int> { 6, 4, 5, 3 };

        List<int[]> validRecipes = new List<int[]>();
        foreach (int[] recipe in recipeScript.RecipeArray)
        {
            bool isValidRecipe = true;
            foreach (int num in recipe)
            {
                if (!ingreIDArr.Contains(num))
                {
                    isValidRecipe = false;
                    break;
                }
            }
            if (isValidRecipe)
            {
                validRecipes.Add(recipe);
            }
        }

        foreach (int[] validRecipe in validRecipes)
        {
            GameObject RecipePrefabObj = Instantiate(RecipePrefab);
            RecipePrefabObj.transform.SetParent(RecipeContent.transform);


            for (int a = 0; a < items.Items.Length; a++)
            {
                for(int b = 0; b < 4; b++)
                {
                    if(validRecipe[b] == items.Items[a].Data.ID)
                    {
                        RecipePrefab.transform.GetChild(b).GetComponent<Image>().sprite = items.Items[a].UiDisplay;
                    }
                }
            }
        }
        Debug.Log("생성 끝");
    }
}