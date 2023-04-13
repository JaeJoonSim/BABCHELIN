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


    //오브젝트 선언
    GameObject CarriageUI;              //마차 UI
    GameObject NowOrderImage;           //플레이어가 지금 처음 만들어야 하는 주문
    GameObject InputKeyImgObject;       //플레이어가 누르는 키 이미지
    public GameObject OrderPrefab;      //주문 표 프리팹
    GameObject OrderGrid;               //주문 표 프리팹 나열 Grid 적용 오브젝트
    GameObject orderObject;             //프리팹 생성용 오브젝트
    Slider timeGaugeBar;                //현재 주문 시간제한용 Slider 오브젝트
    TMP_Text TimeLimitText;             //마차 전체 오픈 시간 Text
    GameObject AccelerantUI;            //오감촉진제 부여 UI
    Slider AccelerantPointer;           //오감 부여 화살표


    //스크립트
    CarriageScript carriageScript;


    List<int> OrderNumArray;                    //주문된 주문의 번호
    int[] nowOrderNumArray;
    List<int> QWERImageNum;                 //키 입력 이미지용 리스트
    List<int> inputList;                    //키 입력시 반환되는 숫자 리스트
    int inputKeyNum;
    bool isSuccess;
    private float maxOrderTime = 10f;
    float openTime;
    bool isBlinking;
    int blinkCount;
    float minValue;
    float maxValue;
    float timeLimit;
    bool checkUpDown;
    bool accelGameStart;

    // Start is called before the first frame update
    void Start()
    {
        //오브젝트 할당
        CarriageUI = this.transform.parent.GetChild(0).gameObject;
        NowOrderImage = this.transform.GetChild(1).transform.GetChild(0).gameObject;
        InputKeyImgObject = this.transform.GetChild(2).gameObject;
        OrderGrid = this.transform.GetChild(1).transform.GetChild(1).gameObject;
        timeGaugeBar = NowOrderImage.transform.GetChild(5).GetComponent<Slider>();
        TimeLimitText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        AccelerantUI = this.transform.GetChild(3).gameObject;
        AccelerantPointer = AccelerantUI.transform.GetChild(1).GetComponent<Slider>();


        //스크립트 할당
        carriageScript = CarriageUI.GetComponent<CarriageScript>();


        OrderNumArray = new List<int>();                        //주문된 주문의 번호
        nowOrderNumArray = new int[] { -1, -1, -1, -1 };        //지금 주문된 레시피 번호 리스트
        QWERImageNum = new List<int>();
        inputList = new List<int>();

        openTime = 180f;

        timeGaugeBar.maxValue = maxOrderTime;

        isBlinking = false;
        blinkCount = 0;

        minValue = 0;
        maxValue = 0;
        timeLimit = 0f;
        checkUpDown = true;
        accelGameStart = false;

        RecipeImageSetting();       //임시 함수
        QWERNumSetting();
    }

    // Update is called once per frame
    void Update()
    {
        TimeLimitText.text = Mathf.Round(openTime).ToString();
        CookingTimeLimit();
        QWERImageSetting();
        PlayerInputKey();
        MoveAccelPointer();


        RecipeTimeLimit();
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
            OrderReceipt();
        }
    }

    void CookingComplete()
    {
        if(isSuccess)
        {
            //돈 올라가는 부분 작업 예정
            MoneyScript.moneyGold += 1000;
        }

        NowOrderImage.transform.GetChild(0).GetComponent<Image>().sprite = OrderGrid.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite;

        for (int a = 0; a < 3; a++)
        {
            OrderGrid.transform.GetChild(a).transform.GetChild(0).GetComponent<Image>().sprite = OrderGrid.transform.GetChild(a + 1).transform.GetChild(0).GetComponent<Image>().sprite;
        }
        for(int a = 0; a < 4; a++)
        {
            Debug.Log(OrderNumArray[a]);
            OrderNumArray[a] = OrderNumArray[a + 1];
        }

        DestroyLastOrder();
        OrderReceipt();

        Shuffle(QWERImageNum);

        timeGaugeBar.value = maxOrderTime;
    }

    void QWERNumSetting()           //QWERT 키에 각각 값 할당
    {
        for(int a = 0; a < 4; a++)
        {
            QWERImageNum.Add(carriageScript.ingreIDArray[a]);
        }
        QWERImageNum.Add(-1);

        Shuffle(QWERImageNum);
    }

    void QWERImageSetting()         //QWERT 키에 각각 이미지 할당
    {
        for(int a = 0; a < 5; a++)
        {
            if(QWERImageNum[a] == -1)
            {
                InputKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(0f, 0f, 0f, 40f);      //빈칸 이미지 소스 나오기 전 임시 코드
            }
            else
            {
                InputKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(1f, 1f, 1f, 40f);
                InputKeyImgObject.transform.GetChild(a).GetComponent<Image>().sprite = items.Items[QWERImageNum[a]].UiDisplay;
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
        if (nowOrderNumArray[inputKeyNum] == inputList[inputKeyNum])
        {
            NowOrderImage.transform.GetChild(inputKeyNum + 1).GetChild(0).gameObject.SetActive(true);
            inputKeyNum++;

            // 정답 리스트를 모두 맞춘 경우
            if (inputKeyNum == nowOrderNumArray.Length)
            {
                isSuccess = true;
                AccelerantGameStart();
                //accelGameStart = true;
                CheckImageReset(); // 체크 이미지 초기화
                inputList.Clear(); // 플레이어 입력 리스트 초기화
                inputKeyNum = 0; // 현재 검사할 정답 리스트의 인덱스 초기화
                CookingComplete();
            }
        }
        else
        {
            timeGaugeBar.value -= 2;
            Shuffle(QWERImageNum);      //입력 이미지 섞기
            InvokeRepeating("BlinkImage", 0.3f, 0.3f);      //이미지 깜빡
            CheckImageReset();          //체크 이미지 초기화
            inputList.Clear(); // 플레이어 입력 리스트 초기화
            inputKeyNum = 0; // 현재 검사할 정답 리스트의 인덱스 초기화
        }
    }

    void CheckImageReset()
    {
        for(int a = 0; a < 4; a++)
        {
            NowOrderImage.transform.GetChild(a + 1).GetChild(0).gameObject.SetActive(false);
        }
    }

    void OrderReceipt()             //주문 접수
    {
        OrderNumArray.Add(Random.Range(0, carriageScript.FoodNumArray.Count));
        if (OrderNumArray.Count == 1)
        {
            NowOrderImage.SetActive(true);
            NowOrderImage.transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[OrderNumArray[0]] + 1].UiDisplay;
            nowOrderNumArray = carriageScript.AbleRecipeNumArray[OrderNumArray[0]];
            for (int a = 0; a < nowOrderNumArray.Length; a++)
            {
                NowOrderImage.transform.GetChild(a + 1).GetComponent<Image>().sprite = items.Items[nowOrderNumArray[a]].UiDisplay;
            }
        }
        else if(OrderNumArray.Count > 1)
        {
            orderObject = Instantiate(OrderPrefab);
            orderObject.transform.SetParent(OrderGrid.transform);
            orderObject.transform.GetChild(0).GetComponent<Image>().sprite = items.Items[carriageScript.FoodNumArray[OrderNumArray[OrderNumArray.Count - 1]] + 1].UiDisplay;
        }
    }
    void DestroyLastOrder()
    {
        Destroy(OrderGrid.transform.GetChild(OrderGrid.transform.childCount - 1).gameObject);
        OrderNumArray.RemoveAt(OrderNumArray.Count - 1);
    }

    void AccelerantGameStart()
    {
        InputKeyImgObject.SetActive(false);
        AccelerantUI.SetActive(true);
        int AccelSpotNum = Random.Range(0, 5);

        GameObject TargetImg = AccelerantUI.transform.GetChild(0).transform.GetChild(0).gameObject;
        switch (AccelSpotNum)
        {
            case 0:
                TargetImg.GetComponent<Transform>().localPosition = new Vector3(60f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(150f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Transform>().localPosition = new Vector3(210f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Transform>().localPosition = new Vector3(270f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Transform>().localPosition = new Vector3(330f, 0f, 0f);
                minValue = 0f;
                maxValue = 2f;
                break;
            case 1:
                AccelerantUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(30f, 0f, 0f);
                TargetImg.GetComponent<Transform>().localPosition = new Vector3(120f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Transform>().localPosition = new Vector3(210f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Transform>().localPosition = new Vector3(270f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Transform>().localPosition = new Vector3(330f, 0f, 0f);
                minValue = 1;
                maxValue = 3;
                break;
            case 2:
                AccelerantUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(30f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Transform>().localPosition = new Vector3(90f, 0f, 0f);
                TargetImg.GetComponent<Transform>().localPosition = new Vector3(180f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Transform>().localPosition = new Vector3(270f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Transform>().localPosition = new Vector3(330f, 0f, 0f);
                minValue = 2;
                maxValue = 4;
                break;
            case 3:
                AccelerantUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(30f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Transform>().localPosition = new Vector3(90f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Transform>().localPosition = new Vector3(150f, 0f, 0f);
                TargetImg.GetComponent<Transform>().localPosition = new Vector3(240f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Transform>().localPosition = new Vector3(330f, 0f, 0f);
                minValue = 3;
                maxValue = 5;
                break;
            case 4:
                AccelerantUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(30f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Transform>().localPosition = new Vector3(90f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Transform>().localPosition = new Vector3(150f, 0f, 0f);
                AccelerantUI.transform.GetChild(0).transform.GetChild(4).GetComponent<Transform>().localPosition = new Vector3(210f, 0f, 0f);
                TargetImg.GetComponent<Transform>().localPosition = new Vector3(300f, 0f, 0f);
                minValue = 4;
                maxValue = 6;
                break;
        }

        accelGameStart = true;
    }


    void MoveAccelPointer()     //오감 촉진제 게임
    {
        if (AccelerantPointer.value <= 0)
        {
            checkUpDown = true;
        }
        else if (AccelerantPointer.value >= 6)
        {
            checkUpDown = false;
        }
        if (checkUpDown)
        {
            AccelerantPointer.value += (Time.deltaTime * 3f);
        }
        else if (!checkUpDown)
        {
            AccelerantPointer.value -= (Time.deltaTime * 3f);
        }



        if (accelGameStart)             //
        {
            timeLimit += Time.deltaTime;

            if(timeLimit < 5f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (minValue <= AccelerantPointer.value && AccelerantPointer.value <= maxValue)
                    {
                        //부여 성공시 
                        Debug.Log("성공");

                    }
                    else
                    {
                        //부여 실패시
                        Debug.Log("실패");
                    }
                    timeLimit = 0f;
                    InputKeyImgObject.SetActive(true);
                    AccelerantUI.SetActive(false);
                    accelGameStart = false;
                }
            }
            else
            {
                //부여 실패시
                Debug.Log("실패");
                timeLimit = 0f;
                InputKeyImgObject.SetActive(true);
                AccelerantUI.SetActive(false);
                accelGameStart = false;
            }
        }
    }


    void RecipeTimeLimit()
    {
        if(!accelGameStart)
        {
            if (timeGaugeBar.value > 0)
            {
                timeGaugeBar.value -= Time.deltaTime;
            }
            else
            {
                isSuccess = false;
                CookingComplete();
            }
        }
        else
        {
            timeGaugeBar.value = timeGaugeBar.value;
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

    void BlinkImage()       //이미지 깜빡거리는 함수
    {
        if (2 <= blinkCount) //4번 깜빡
        {
            CancelInvoke("BlinkImage");
            blinkCount = 0;
        }

        isBlinking = !isBlinking; // 반전

        if (!isBlinking)
        {
            NowOrderImage.GetComponent<Image>().color = Color.red;
        }
        else
        {
            NowOrderImage.GetComponent<Image>().color = Color.white;
        }

        blinkCount++;
    }
}