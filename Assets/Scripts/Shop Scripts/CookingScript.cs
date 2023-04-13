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


    //������Ʈ ����
    GameObject CarriageUI;              //���� UI
    GameObject NowOrderImage;           //�÷��̾ ���� ó�� ������ �ϴ� �ֹ�
    GameObject InputKeyImgObject;       //�÷��̾ ������ Ű �̹���
    public GameObject OrderPrefab;      //�ֹ� ǥ ������
    GameObject OrderGrid;               //�ֹ� ǥ ������ ���� Grid ���� ������Ʈ
    GameObject orderObject;             //������ ������ ������Ʈ
    Slider timeGaugeBar;                //���� �ֹ� �ð����ѿ� Slider ������Ʈ
    TMP_Text TimeLimitText;             //���� ��ü ���� �ð� Text
    GameObject AccelerantUI;            //���������� �ο� UI
    Slider AccelerantPointer;           //���� �ο� ȭ��ǥ


    //��ũ��Ʈ
    CarriageScript carriageScript;


    List<int> OrderNumArray;                    //�ֹ��� �ֹ��� ��ȣ
    int[] nowOrderNumArray;
    List<int> QWERImageNum;                 //Ű �Է� �̹����� ����Ʈ
    List<int> inputList;                    //Ű �Է½� ��ȯ�Ǵ� ���� ����Ʈ
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
        //������Ʈ �Ҵ�
        CarriageUI = this.transform.parent.GetChild(0).gameObject;
        NowOrderImage = this.transform.GetChild(1).transform.GetChild(0).gameObject;
        InputKeyImgObject = this.transform.GetChild(2).gameObject;
        OrderGrid = this.transform.GetChild(1).transform.GetChild(1).gameObject;
        timeGaugeBar = NowOrderImage.transform.GetChild(5).GetComponent<Slider>();
        TimeLimitText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        AccelerantUI = this.transform.GetChild(3).gameObject;
        AccelerantPointer = AccelerantUI.transform.GetChild(1).GetComponent<Slider>();


        //��ũ��Ʈ �Ҵ�
        carriageScript = CarriageUI.GetComponent<CarriageScript>();


        OrderNumArray = new List<int>();                        //�ֹ��� �ֹ��� ��ȣ
        nowOrderNumArray = new int[] { -1, -1, -1, -1 };        //���� �ֹ��� ������ ��ȣ ����Ʈ
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

        RecipeImageSetting();       //�ӽ� �Լ�
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
        for (int a = 0; a < 5; a++)        //�ֹ� ������ ������ŭ ����
        {
            OrderReceipt();
        }
    }

    void CookingComplete()
    {
        if(isSuccess)
        {
            //�� �ö󰡴� �κ� �۾� ����
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

    void QWERNumSetting()           //QWERT Ű�� ���� �� �Ҵ�
    {
        for(int a = 0; a < 4; a++)
        {
            QWERImageNum.Add(carriageScript.ingreIDArray[a]);
        }
        QWERImageNum.Add(-1);

        Shuffle(QWERImageNum);
    }

    void QWERImageSetting()         //QWERT Ű�� ���� �̹��� �Ҵ�
    {
        for(int a = 0; a < 5; a++)
        {
            if(QWERImageNum[a] == -1)
            {
                InputKeyImgObject.transform.GetChild(a).GetComponent<Image>().color = new Color(0f, 0f, 0f, 40f);      //��ĭ �̹��� �ҽ� ������ �� �ӽ� �ڵ�
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

            // ���� ����Ʈ�� ��� ���� ���
            if (inputKeyNum == nowOrderNumArray.Length)
            {
                isSuccess = true;
                AccelerantGameStart();
                //accelGameStart = true;
                CheckImageReset(); // üũ �̹��� �ʱ�ȭ
                inputList.Clear(); // �÷��̾� �Է� ����Ʈ �ʱ�ȭ
                inputKeyNum = 0; // ���� �˻��� ���� ����Ʈ�� �ε��� �ʱ�ȭ
                CookingComplete();
            }
        }
        else
        {
            timeGaugeBar.value -= 2;
            Shuffle(QWERImageNum);      //�Է� �̹��� ����
            InvokeRepeating("BlinkImage", 0.3f, 0.3f);      //�̹��� ����
            CheckImageReset();          //üũ �̹��� �ʱ�ȭ
            inputList.Clear(); // �÷��̾� �Է� ����Ʈ �ʱ�ȭ
            inputKeyNum = 0; // ���� �˻��� ���� ����Ʈ�� �ε��� �ʱ�ȭ
        }
    }

    void CheckImageReset()
    {
        for(int a = 0; a < 4; a++)
        {
            NowOrderImage.transform.GetChild(a + 1).GetChild(0).gameObject.SetActive(false);
        }
    }

    void OrderReceipt()             //�ֹ� ����
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


    void MoveAccelPointer()     //���� ������ ����
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
                        //�ο� ������ 
                        Debug.Log("����");

                    }
                    else
                    {
                        //�ο� ���н�
                        Debug.Log("����");
                    }
                    timeLimit = 0f;
                    InputKeyImgObject.SetActive(true);
                    AccelerantUI.SetActive(false);
                    accelGameStart = false;
                }
            }
            else
            {
                //�ο� ���н�
                Debug.Log("����");
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

    void BlinkImage()       //�̹��� �����Ÿ��� �Լ�
    {
        if (2 <= blinkCount) //4�� ����
        {
            CancelInvoke("BlinkImage");
            blinkCount = 0;
        }

        isBlinking = !isBlinking; // ����

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