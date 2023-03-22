using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class DeliveryScript : MonoBehaviour
{
    public Image IngreImage;
    public TMP_Text IngreName;
    public TMP_Text IngrePrice;
    public GameObject BasketUI;
    int piecePrice;

    public TMP_Text TotalPrice;
    public TMP_InputField PieceInput;
    public Slider PieceSlider;

    [Tooltip("재료아이템 목록")]
    [SerializeField]
    private Item[] items;
    public Item[] Items { get { return items; } }

    public Button button;
    public GameObject ListPanel;
    public GameObject BasketPanel;

    // Start is called before the first frame update
    void Start()
    {
        PieceInput.text = "0";

        PieceSlider.onValueChanged.AddListener(OnSliderChanged);
        PieceInput.onValueChanged.AddListener(OnFieldChanged);
        SetBtn();
    }

    // Update is called once per frame
    void Update()
    {
        Basket();
    }

    void IngreSet()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        IngreImage.sprite = clickObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        IngreName.text = clickObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
        piecePrice = int.Parse(clickObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text);

        BasketUI.SetActive(true);
    }

    void Basket()       //장바구니 추가 함수
    {
        if (string.IsNullOrEmpty(PieceInput.text))
        {
            PieceInput.text = "0";
        }
        
        if(int.Parse(PieceInput.text) > 99)
        {
            PieceInput.text = 99.ToString();
        }

        if (int.Parse(PieceInput.text) > MoneyScript.moneyGold)
        {
            int p = MoneyScript.moneyGold / piecePrice;
            PieceInput.text = p.ToString();
        }

        TotalPrice.text = (piecePrice * int.Parse(PieceInput.text)).ToString();
    }

    public void CloseBtn()
    {
        BasketUI.SetActive(false);
    }
    public void AddBtn()
    {
        BasketUI.SetActive(false);
    }

    private void OnSliderChanged(float number)      //InputField - Slider 연동용 함수
    {
        if (PieceInput.text != number.ToString())
        {
            PieceInput.text = number.ToString();
        }
    }

    private void OnFieldChanged(string text)        //InputField - Slider 연동용 함수
    {
        if (PieceSlider.value.ToString() != text)
        {
            if (float.TryParse(text, out float number))
            {
                PieceSlider.value = number;
            }
        }
    }

    public void SetBtn()        //버튼 생성 함수
    {
        for(int a = 0; a < Items.Length; a++)
        {
            Button BtnList = Instantiate(button);
            BtnList.onClick.AddListener(IngreSet);

            BtnList.transform.GetChild(0).GetComponent<Image>().sprite = Items[a].UiDisplay;
            BtnList.transform.GetChild(1).GetComponent<TMP_Text>().text = Items[a].Data.Name;
            BtnList.transform.GetChild(2).GetComponent<TMP_Text>().text = Items[a].Data.PRICE.ToString();

            BtnList.transform.SetParent(ListPanel.transform);
        }
    }

    public void AddToCart()
    {

    }
}
