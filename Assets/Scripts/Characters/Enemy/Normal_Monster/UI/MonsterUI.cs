using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] Transform monster;
    private Health monsterHealth;
    private PlayerController playerController;

    public Image UIHealthGauge;
    public Image UIHealthBackground;
    private Color HPBarColor;
    private bool fadeout;
    private float time;

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

        if (monster.localScale.x < 0)
        {
            UIHealthGauge.gameObject.transform.localScale = new Vector3(-0.004f, 0.004f, 0.004f);
        }
        else
        {
            UIHealthGauge.gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
        }

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
            time = 0;
            fadeout = true;
        }
    }

    private void FadeoutHPBar()
    {
        time += Time.deltaTime;

        if(time >= 2)
        {
            HPBarColor.a -= Time.deltaTime * 2;
        }
    }
}