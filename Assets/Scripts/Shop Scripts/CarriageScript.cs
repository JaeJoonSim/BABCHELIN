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


    [Tooltip("������")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    [Tooltip("�丮�۾���")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    public GameObject RecipeUI;

    [HideInInspector]
    public List<int> ingreIDArray;                                  //���̺� �ø� ������ ID
    public List<int> FoodNumArray = new List<int>();                //�ϼ��丮 ��ȣ ���� ����Ʈ
    public List<int[]> AbleRecipeNumArray = new List<int[]>();      //�ø� ���� �丮 ������ ������ ����Ʈ

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
            if (cook.Items.Items[0].Item.ID == -1)  //���� ��� ����
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;
                isReady = false;
            }
            else if (Cook.Items.Items[1].Item.ID == -1 || Cook.Items.Items[2].Item.ID == -1 || Cook.Items.Items[3].Item.ID == -1)   //���� ��� ����
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

            RecipePrefabObj.transform.GetChild(4).GetComponent<Image>().sprite = items.Items[FoodNumArray[recipeNum] + 1].UiDisplay;     //�丮�� ���� ������ �����ͺ��̽���  recipeNum+1��° �������� �̹��� ����       //*�̿ϼ�  �ϼ��丮 �����ͺ��̽� �۾� �ʿ�.  items -> FoodItem ������ �ٲ� ����
            for (int a = 0; a < items.Items.Length; a++)    //��� ������ �����ͺ��̽� ID �� �� �����ǿ� ��� �̹��� ����
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