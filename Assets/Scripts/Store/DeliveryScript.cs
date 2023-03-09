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

    int t;

    // Start is called before the first frame update
    void Start()
    {
        PieceInput.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        Basket();
    }

    public void IngreSet(int a)
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        IngreImage.sprite = clickObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        IngreName.text = clickObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
        piecePrice = int.Parse(clickObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text);

        BasketUI.SetActive(true);
    }

    void Basket()
    {
        if (string.IsNullOrEmpty(PieceInput.text))
        {
            PieceInput.text = "0";
        }
        
        if(int.Parse(PieceInput.text) > 99)
        {
            PieceInput.text = 99.ToString();
        }

        PieceSlider.value = int.Parse(PieceInput.text);
        //if(PieceSlider.value < int.Parse(PieceInput.text))
        //{
        //    PieceInput.text = ((int)PieceSlider.value).ToString();
        //}
        //else
        //{
        //    PieceSlider.value = int.Parse(PieceInput.text);
        //}

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
}
