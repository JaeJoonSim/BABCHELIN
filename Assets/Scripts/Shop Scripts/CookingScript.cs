using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CookingScript : MonoBehaviour
{
    [Tooltip("아이템")]
    [SerializeField]
    private ItemDatabase items;
    public ItemDatabase Items { get { return items; } }

    float openTime;

    public TMP_Text TimeLimitText;
    public GameObject CarriageObj;
    CarriageScript carriageScript;

    public GameObject[] RecipeObject;
    public GameObject RecipeKeyImgObject;
    //이거 말고 차라리 NowRecipeObject 한개와 나머지 4개를 따로 선언하고 현재꺼를 클리어하면 NowRecipeObject 삭제 후 재생성 + 한칸씩 당기기...
    int[] recipeNumArray;
    int[] nowRecipeNumArray;

    List<int> QWERImageNum;     //키 입력 이미지용 리스트
    List<int> inputList;        //키 입력시 반환되는 숫자 리스트
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
        for (int a = 0; a < 5; a++)        //주문 레시피 개수만큼 루프
        {
            recipeNumArray[a] = Random.Range(0, carriageScript.FoodNumArray.Count);             //손님이 주문하는 것 랜덤으로 임시 구현. 손님 AI로 대체될 듯
            //RecipeObject[a].transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[a]] + 1].UiDisplay;     //요리된 음식 아이템 데이터베이스의  recipeNum+1번째 아이템의 이미지 띄우기       //*미완성  완성요리 데이터베이스 작업 필요.  items -> FoodItem 식으로 바꿀 예정

            if (a == 0)
            {
                RecipeObject[a].transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[recipeNumArray[a]] + 1].UiDisplay;     //요리된 음식 아이템 데이터베이스의  recipeNum+1번째 아이템의 이미지 띄우기       //*미완성  완성요리 데이터베이스 작업 필요.  items -> FoodItem 식으로 바꿀 예정
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

        //for (int a = 0; a < items.Items.Length; a++)    //재료 아이템 데이터베이스 ID 비교 후 레시피에 재료 이미지 띄우기
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
        //돈 올라가는 부분 작업 예정

        RecipeObject[0].transform.GetChild(0).GetComponent<Image>().sprite = OrderGrid.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite;

        for (int a = 0; a < 3; a++)
        {
            Debug.Log("이미지 변경");
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
                RecipeKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(0f, 0f, 0f, 40f);      //빈칸 채우기 소스 나오기 전 임시 코드
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

            // 정답 리스트를 모두 맞춘 경우
            if (inputKeyNum == nowRecipeNumArray.Length)
            {
                inputList.Clear(); // 플레이어 입력 리스트 초기화
                inputKeyNum = 0; // 현재 검사할 정답 리스트의 인덱스 초기화
                CookingComplete();
            }
        }
        else
        {
            inputList.Clear(); // 플레이어 입력 리스트 초기화
            inputKeyNum = 0; // 현재 검사할 정답 리스트의 인덱스 초기화
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