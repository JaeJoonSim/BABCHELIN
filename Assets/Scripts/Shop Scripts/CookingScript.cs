using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CookingScript : MonoBehaviour
{
    [Tooltip("������")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    float openTime;

    public TMP_Text TimeLimitText;
    public GameObject CarriageObj;
    CarriageScript carriageScript;

    public GameObject[] RecipeObject;
    public GameObject RecipeKeyImgObject;
    //�̰� ���� ���� NowRecipeObject �Ѱ��� ������ 4���� ���� �����ϰ� ���粨�� Ŭ�����ϸ� NowRecipeObject ���� �� ����� + ��ĭ�� ����...
    int[] recipeNumArray;
    int[] nowRecipeNumArray;

    List<int> QWERImageNum;     //Ű �Է� �̹����� ����Ʈ
    List<int> inputList;        //Ű �Է½� ��ȯ�Ǵ� ���� ����Ʈ
    int inputKeyNum;
    bool isSuccess;

    public GameObject OrderPrefab;
    public GameObject OrderGrid;
    GameObject orderObject;
    //public GameObject RecipePrefab;
    //GameObject RecipePrefabObj;

    // Start is called before the first frame update
    void Start()
    {
        recipeNumArray = new int[] { -1, -1, -1, -1, -1 };
        nowRecipeNumArray = new int[] { -1, -1, -1, -1 };
        QWERImageNum = new List<int>();
        inputList = new List<int>();

        //nowRecipeNumArray = new int[] { -1, -1, -1, -1 };
        openTime = 180f;
        carriageScript = CarriageObj.GetComponent<CarriageScript>();
        RecipeImageSetting();

        QWERNumSetting();
    }

    // Update is called once per frame
    void Update()
    {
        TimeLimitText.text = Mathf.Round(openTime).ToString();
        CookingTimeLimit();
        QWERImageSetting();
        PlayerInputKey();
    }

    void CookingTimeLimit()
    {
        openTime -= Time.deltaTime;

        if (openTime <= 0)
        {
            openTime = 0;
            this.gameObject.SetActive(false);
        }
    }

    void RecipeImageSetting()
    {
        for (int a = 0; a < 5; a++)        //�ֹ� ������ ������ŭ ����
        {
            recipeNumArray[a] = Random.Range(0, carriageScript.FoodNumArray.Count);             //�մ��� �ֹ��ϴ� �� �������� �ӽ� ����. �մ� AI�� ��ü�� ��
            //RecipeObject[a].transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[a]] + 1].UiDisplay;     //�丮�� ���� ������ �����ͺ��̽���  recipeNum+1��° �������� �̹��� ����       //*�̿ϼ�  �ϼ��丮 �����ͺ��̽� �۾� �ʿ�.  items -> FoodItem ������ �ٲ� ����

            if (a == 0)
            {
                RecipeObject[a].transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[a]] + 1].UiDisplay;     //�丮�� ���� ������ �����ͺ��̽���  recipeNum+1��° �������� �̹��� ����       //*�̿ϼ�  �ϼ��丮 �����ͺ��̽� �۾� �ʿ�.  items -> FoodItem ������ �ٲ� ����
            }
            else
            {
                orderObject = Instantiate(OrderPrefab);
                orderObject.transform.SetParent(OrderGrid.transform);
                orderObject.transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[a]] + 1].UiDisplay;
            }
        }
        
        nowRecipeNumArray = carriageScript.AbleRecipeNumArray[recipeNumArray[0]];

        for(int a = 0; a < nowRecipeNumArray.Length; a++)
        {
            RecipeObject[0].transform.GetChild(a + 1).GetComponent<Image>().sprite = items.Items[nowRecipeNumArray[a]].UiDisplay;
        }

        //for (int a = 0; a < items.Items.Length; a++)    //��� ������ �����ͺ��̽� ID �� �� �����ǿ� ��� �̹��� ����
        //{
        //    for (int b = 0; b < 4; b++)
        //    {
        //        if (carriageScript.AbleRecipeNumArray[recipeNum][b] == items.Items[a].Data.ID)
        //        {
        //            RecipeObject[0].transform.GetChild(b).GetComponent<Image>().sprite = items.Items[a].UiDisplay;
        //        }
        //    }
        //}
    }

    void CookingComplete()
    {
        //�� �ö󰡴� �κ� �۾� ����

        RecipeObject[0].transform.GetChild(0).GetComponent<Image>().sprite = OrderGrid.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite;

        for (int a = 0; a < 3; a++)
        {
            Debug.Log("�̹��� ����");
            OrderGrid.transform.GetChild(a).transform.GetChild(0).GetComponent<Image>().sprite = OrderGrid.transform.GetChild(a + 1).transform.GetChild(0).GetComponent<Image>().sprite;
            //RecipeObject[a].transform.GetChild(0).GetComponent<Image>().sprite = RecipeObject[a + 1].transform.GetChild(0).GetComponent<Image>().sprite;
        }
        for(int a = 0; a < 4; a++)
        {
            Debug.Log(recipeNumArray[a]);
            recipeNumArray[a] = recipeNumArray[a + 1];
        }

        Destroy(OrderGrid.transform.GetChild(3).gameObject);
        orderObject = Instantiate(OrderPrefab);
        orderObject.transform.SetParent(OrderGrid.transform);

        recipeNumArray[4] = Random.Range(0, carriageScript.FoodNumArray.Count);
        OrderGrid.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[4]] + 1].UiDisplay;
        //RecipeObject[4].transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[4]] + 1].UiDisplay;


        nowRecipeNumArray = carriageScript.AbleRecipeNumArray[recipeNumArray[0]];
        for (int a = 0; a < nowRecipeNumArray.Length; a++)
        {
            RecipeObject[0].transform.GetChild(a + 1).GetComponent<Image>().sprite = items.Items[nowRecipeNumArray[a]].UiDisplay;
        }

        Shuffle(QWERImageNum);
    }

    void QWERNumSetting()
    {
        for(int a = 0; a < 4; a++)
        {
            QWERImageNum.Add(carriageScript.ingreIDArray[a]);
        }
        QWERImageNum.Add(-1);

        Shuffle(QWERImageNum);
    }

    void QWERImageSetting()
    {
        for(int a = 0; a < 5; a++)
        {
            if(QWERImageNum[a] == -1)
            {
                RecipeKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(0f, 0f, 0f, 40f);      //��ĭ ä��� �ҽ� ������ �� �ӽ� �ڵ�
            }
            else
            {
                RecipeKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(1f, 1f, 1f, 40f);
                RecipeKeyImgObject.transform.GetChild(a).GetComponent<Image>().sprite = items.Items[QWERImageNum[a]].UiDisplay;
            }
        }
    }

    void PlayerInputKey()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inputList.Add(QWERImageNum[0]);
            CheckOrder();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            inputList.Add(QWERImageNum[1]);
            CheckOrder();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            inputList.Add(QWERImageNum[2]);
            CheckOrder();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            inputList.Add(QWERImageNum[3]);
            CheckOrder();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            inputList.Add(QWERImageNum[4]);
            CheckOrder();
        }
    }

    void CheckOrder()
    {
        if (nowRecipeNumArray[inputKeyNum] == inputList[inputKeyNum])
        {
            inputKeyNum++;

            // ���� ����Ʈ�� ��� ���� ���
            if (inputKeyNum == nowRecipeNumArray.Length)
            {
                inputList.Clear(); // �÷��̾� �Է� ����Ʈ �ʱ�ȭ
                inputKeyNum = 0; // ���� �˻��� ���� ����Ʈ�� �ε��� �ʱ�ȭ
                CookingComplete();
            }
        }
        else
        {
            inputList.Clear(); // �÷��̾� �Է� ����Ʈ �ʱ�ȭ
            inputKeyNum = 0; // ���� �˻��� ���� ����Ʈ�� �ε��� �ʱ�ȭ
        }
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void RecipeTimeLimit()
    {
        
    }

}