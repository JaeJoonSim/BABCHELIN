using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cart : MonoBehaviour
{
    public GameObject Product;
    public TMP_Text ProductNameText;
    public Image ProductImage;
    public Slider ProductCountSlider;
    int productCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (productCount >= 99)
        {
            productCount = 99;
        }
    }

    public void CheckOverlap(TMP_Text name)
    {
        for (int a = 0; a <= this.transform.childCount; a++)
        {
            if (this.transform.GetChild(a).transform.GetChild(1).GetComponent<TMP_Text>().text == name.text)
            {
                GameObject ProductObj = Instantiate(Product);

                ProductObj.transform.GetChild(0).GetComponent<Image>().sprite = ProductImage.sprite;
                ProductObj.transform.GetChild(1).GetComponent<TMP_Text>().text = ProductNameText.text;
                ProductObj.transform.GetChild(3).GetComponent<TMP_Text>().text = ProductCountSlider.value.ToString();

                productCount = (int)ProductCountSlider.value;
                ProductObj.transform.GetChild(3).GetComponent<TMP_Text>().text = productCount.ToString();

                ProductObj.transform.SetParent(this.transform);
            }
            else
            {
                GameObject ProductObj = Instantiate(Product);

                ProductObj.transform.GetChild(0).GetComponent<Image>().sprite = ProductImage.sprite;
                ProductObj.transform.GetChild(1).GetComponent<TMP_Text>().text = ProductNameText.text;
                ProductObj.transform.GetChild(3).GetComponent<TMP_Text>().text = ProductCountSlider.value.ToString();

                productCount = (int)ProductCountSlider.value;
                ProductObj.transform.GetChild(3).GetComponent<TMP_Text>().text = productCount.ToString();

                ProductObj.transform.SetParent(this.transform);
            }
        }
    }

}
