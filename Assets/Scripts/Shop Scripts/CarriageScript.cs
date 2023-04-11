using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriageScript : MonoBehaviour
{
    [Tooltip("������")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    [Tooltip("��� �ø��� ��")]
    [SerializeField]
    private Inventory cook;
    public Inventory Cook { get { return cook; } }

    [Tooltip("��� �ø��� ��")]
    [SerializeField]
    private Inventory playerInventory;
    public Inventory PlayerInventory { get { return playerInventory; } }


    //������Ʈ ����
    public GameObject Manager;
    Button OpenBtn;
    ColorBlock OpenBtnColor;
    GameObject RecipeUI;
    GameObject RecipeContent;
    public GameObject RecipePrefab;
    GameObject RecipePrefabObj;
    GameObject CookingUI;


    //��ũ��Ʈ
    TimeScript timeScript;
    RecipeScript recipeScript;


    [HideInInspector]
    public List<int> ingreIDArray;                                  //���̺� �ø� ������ ID
    public List<int> FoodNumArray = new List<int>();                //�ϼ��丮 ��ȣ ���� ����Ʈ
    public List<int[]> AbleRecipeNumArray = new List<int[]>();      //�ø� ���� �丮 ������ ������ ����Ʈ

    bool ingreReady = false;
    bool recipeReady = false;


    private void Start()
    {
        //������Ʈ �Ҵ�
        OpenBtn = this.transform.GetChild(2).GetComponent<Button>();
        OpenBtnColor = OpenBtn.colors;
        RecipeUI = this.transform.GetChild(3).GetChild(0).gameObject;
        RecipeContent = RecipeUI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        CookingUI = this.transform.parent.GetChild(1).gameObject;


        //��ũ��Ʈ �Ҵ�
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
        if (timeScript.isNight == true) //���� �� ��� �� ���� �� ��ư Ȱ��ȭ
        {
            if (cook.Items.Items[0].Item.ID == -1)  //���� ��� ����
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;

                RecipUIReset();
            }
            else if (Cook.Items.Items[1].Item.ID == -1 || Cook.Items.Items[2].Item.ID == -1 || Cook.Items.Items[3].Item.ID == -1)   //���� ��� ����
            {
                OpenBtnColor.normalColor = new Color(100f, 100f, 100f, 210f);
                OpenBtn.interactable = false;

                RecipUIReset();
            }
            else
            {
                for (int a = 0; a < 4; a++)     //���̺� �ø� ������ ID��  ingreIDArray ����Ʈ�� ����
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

            RecipeUI.SetActive(true);

            recipeReady = true;
        }
    }

    void RecipUIReset()         //������ UI �ʱ�ȭ �Լ�
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

    void IngreReset()       //UI �� �� ��� �ٽ� inventory�� �ű�� �Լ�
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