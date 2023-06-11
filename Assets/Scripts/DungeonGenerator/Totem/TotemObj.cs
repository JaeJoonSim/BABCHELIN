using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class TotemObj : MonoBehaviour
{
    private Totem item;
    public string name;
    bool isget = false;

    [Header("툴팁")]
    public GameObject Canvas;
    public TextMeshProUGUI UiName;
    public TextMeshProUGUI UiDescription;
    public Image UiIcon;


    public GameObject[] otherTotem;

    public SpriteRenderer spriteRenderer;

    [Header("시작연출")]
    public GameObject effetObj;
    public GameObject spriteObj;
    public GameObject IconObj;

    [Header("선택연출")]
    public ParticleSystem TotemChooseEffect;
    public ParticleSystem PcTotemChooseEffect;

    [Header("사라짐연출")]
    Vector3 iconPos;
    public ParticleSystem TotemDestroyEffect;

    void Start()
    {
        Invoke("Replace", 1.5f);
        Invoke("getItem", 0.3f);
    }
    private void Update()
    {
        if (isget) 
        {
            Vector3 direction = absorb.Instance.Player.position - IconObj.transform.position;

            IconObj.transform.Translate(direction.normalized * absorb.Instance.speed * Time.deltaTime);

            if(Vector3.Distance(absorb.Instance.Player.position , IconObj.transform.position) < 1f)
            {
                Instantiate(TotemDestroyEffect, iconPos, quaternion.identity);
                Destroy(gameObject);
            }
        }

        if (Vector3.Distance(absorb.Instance.Player.position, transform.position) < 2f)
        {

            Canvas.SetActive(true);
        }
        else
        {
            //var stat = absorb.Instance.Player.gameObject.GetComponent<PlayerController>().TotalStatus.variables[];
            Canvas.SetActive(false);
        }
                
    }
    void Replace()
    {
        effetObj.SetActive(false);
        spriteObj.SetActive(true);
        IconObj.SetActive(true);
    }
    void getItem()
    {
        item = TotemManager.Instance.getTotem();

        spriteRenderer.sprite = TotemManager.Instance.Icons[item.Item];

        name = item.Name;
        UiName.text = item.Name;
        UiDescription.text = item.Description;
        UiIcon.sprite = TotemManager.Instance.Icons[item.Item];
    }
    public void setItmeToPlayer()
    {
        if (isget)
            return;

        isget = true;
        IconObj.GetComponent<FloatingObject>().ismoving = false;
        iconPos = IconObj.transform.position;
        TotemManager.Instance.isAdd[item.Type]= item;
        absorb.Instance.Player.gameObject.GetComponent<PlayerController>().addItem();

        Instantiate(TotemChooseEffect, IconObj.transform.position , quaternion.identity, transform);
        Instantiate(PcTotemChooseEffect, absorb.Instance.Player.position, quaternion.identity, absorb.Instance.Player);

        for (int i = 0; i < otherTotem.Length; i++)
        {
            if (otherTotem[i] != null)
                Destroy(otherTotem[i]);
        }

    }

}
