using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Basket : MonoBehaviour
{
    public GameObject CartObj;
    Cart CartScript;

    public TMP_Text IngreName;
    public TMP_InputField PieceInput;
    public Slider PieceSlider;
    public TMP_Text TotalPrice;
    int piecePrice;

    // Start is called before the first frame update
    void Start()
    {
        CartScript = CartObj.GetComponent<Cart>();

        PieceSlider.onValueChanged.AddListener(OnSliderChanged);
        PieceInput.onValueChanged.AddListener(OnFieldChanged);

        piecePrice = int.Parse(TotalPrice.text);
    }

    // Update is called once per frame
    void Update()
    {
        Information();
    }

    void Information()       //장바구니 추가 함수
    {
        if (string.IsNullOrEmpty(PieceInput.text))  //최소값 1
        {
            PieceInput.text = "1";
        }

        if (int.Parse(PieceInput.text) > 99)        //최대값 99
        {
            PieceInput.text = "99";
        }

        //if (int.Parse(PieceInput.text) > MoneyScript.moneyGold)
        //{
        //    int p = MoneyScript.moneyGold / piecePrice;
        //    PieceInput.text = p.ToString();
        //}

        TotalPrice.text = (piecePrice * int.Parse(PieceInput.text)).ToString();
    }

    public void AddToCart()
    {
        CartScript.CheckOverlap(IngreName);

        this.gameObject.SetActive(false);
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

}
