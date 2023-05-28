using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] Transform boss;
    private Health bossHealth;
    private Skunk skunk;
    public Image UIHealthGauge;
    public Slider UIHealthCutline;
    public Image UIDestroyGauge;
    //public Image UIHealthBackGauge;

    // Start is called before the first frame update
    void Start()
    {
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("boss").transform;
        }
        bossHealth = boss.GetComponent<Health>();
        skunk = boss.GetComponent<Skunk>();

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
        UIDestroyGauge.fillAmount = skunk.destructionGauge() / 5;
        //UIHealthBackGauge.fillAmount = playerHealth.BackHP() / playerHealth.MaxHP();
    }
}