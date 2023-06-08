using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    [SerializeField] Transform boss;
    private Health bossHealth;
    private Skunk skunk;

    public TMP_Text bossName;
    public Image UIHealthGauge;
    public Slider UIHealthCutline;
    public Image UIDestroyGauge;
    public Slider UIDestroyGaugeCutline;
    //public Image UIHealthBackGauge;

    private float destroyGauge;
    private float destroyGaugeCutline;

    // Start is called before the first frame update
    void Start()
    {
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("boss").transform;
        }
        bossName.text = boss.name;
        bossHealth = boss.GetComponent<Health>();
        skunk = boss.GetComponent<Skunk>();

        destroyGauge = skunk.destructionGauge();
        destroyGaugeCutline = skunk.destructionGauge();

        //DefaultUltIcon = PlayerUIBackground.GetChild(4).GetComponent<Image>();
        //ActiveUltIcon = PlayerUIBackground.GetChild(5).GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
    }

    private void GaugeManagement()
    {
        UIHealthGauge.fillAmount = bossHealth.CurrentHP() / bossHealth.MaxHP();
        UIHealthCutline.value = bossHealth.CurrentHP() / bossHealth.MaxHP();
        UIDestroyGauge.fillAmount = skunk.destructionGauge() / destroyGauge;
        UIDestroyGaugeCutline.value = skunk.destructionGauge() / destroyGaugeCutline;
        //UIHealthBackGauge.fillAmount = playerHealth.BackHP() / playerHealth.MaxHP();
    }
}