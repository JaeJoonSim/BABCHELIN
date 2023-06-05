using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MonsterUI : MonoBehaviour
{
    [SerializeField] Transform monster;
    private Health monsterHealth;

    public Image UIHealthGauge;
    public Image UIHealthBackground;
    private Color HPBarColor;

    private float hpTime;

    // Start is called before the first frame update
    void Start()
    {
        monsterHealth = monster.GetComponent<Health>();
        HPBarColor = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
        ShowHPBar();

        UIHealthGauge.gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);

        if (HPBarColor.a > 0)
        {
            FadeoutHPBar();
        }
    }

    private void GaugeManagement()
    {
        UIHealthGauge.fillAmount = monsterHealth.CurrentHP() / monsterHealth.MaxHP();
    }

    private void ShowHPBar()
    {
        UIHealthGauge.color = HPBarColor;
        UIHealthBackground.color = HPBarColor;

        if (monsterHealth.CurrentHP() != monsterHealth.BackHP())
        {
            HPBarColor.a = 1;
            hpTime = 0;
        }
    }

    private void FadeoutHPBar()
    {
        hpTime += Time.deltaTime;

        if(hpTime >= 2)
        {
            HPBarColor.a -= Time.deltaTime * 2;
        }
    }
}