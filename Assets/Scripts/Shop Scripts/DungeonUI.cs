using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{

    [SerializeField] Transform player;

    private Health playerHealth;

    Slider PlayerHP;
    Slider BulletGauge;
    Image UltimateGauge;

    // Start is called before the first frame update
    void Start()
    {
        //if (player == null)
        //{
        //    player = GameObject.FindGameObjectWithTag("Player").transform;
        //}
        ////playerHealth = player.GetComponent<HealthPlayer>();

        //PlayerHP = this.transform.GetChild(1).GetComponent<Slider>();
        //BulletGauge = this.transform.GetChild(2).GetComponent<Slider>();
        //PlayerHP.maxValue = player.GetComponent<HealthPlayer>().playerMaxHP();
        //BulletGauge.maxValue = player.GetComponent<HealthPlayer>().playerMaxHP();

        UltimateGauge = this.transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
    }

    private void GaugeManagement()
    {
        //PlayerHP.value = player.GetComponent<HealthPlayer>().playerCurrentHP();             //현재 HP
        //BulletGauge.value = player.GetComponent<HealthPlayer>().playerCurrentHP();          //현재 총알 게이지
        UltimateGauge.fillAmount += Time.deltaTime / 10;
    }
}
