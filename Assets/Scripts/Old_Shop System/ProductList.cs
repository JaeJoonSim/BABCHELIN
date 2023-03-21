using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ProductList : MonoBehaviour
{
    [Tooltip("재료아이템 목록")]
    [SerializeField]
    private Item[] items;
    public Item[] Items { get { return items; } }

    public TMP_Text ProductName;
    public Image ProductImage;
    public TMP_Text ProductPrice;

    public GameObject BasketUI;
    public Button ProductBtnPrefab;

    int[] IDList;       //중복 체크용 ID저장 
    Basket BasketScript;

    // Start is called before the first frame update
    void Start()
    {
        ProductListSetting();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PopUpBasketUI()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        ProductName.text = clickObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
        ProductImage.sprite = clickObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        ProductPrice.text = clickObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;

        BasketUI.SetActive(true);
    }

    public void ProductListSetting()        //물품 목록 버튼 생성 함수
    {
        for (int a = 0; a < Items.Length; a++)
        {
            Button ProductBtn = Instantiate(ProductBtnPrefab);
            ProductBtn.onClick.AddListener(PopUpBasketUI);

            ProductBtn.transform.GetChild(0).GetComponent<Image>().sprite = Items[a].UiDisplay;
            ProductBtn.transform.GetChild(1).GetComponent<TMP_Text>().text = Items[a].Data.Name;
            ProductBtn.transform.GetChild(2).GetComponent<TMP_Text>().text = Items[a].Data.PRICE.ToString();
            ProductBtn.transform.GetChild(1).GetComponent<TMP_Text>().text = Items[a].Data.Name;

            //IDList[a] = Items[a].Data.ID;

            ProductBtn.transform.SetParent(this.transform);
        }
    }
}