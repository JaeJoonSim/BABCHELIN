using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{

    [SerializeField] Transform player;
    private Health playerHealth;

    private GameObject PlayerUI;
    private Image PlayerHPGauge;
    private Image BulletGauge;
    private GameObject PlayerBulletGaugeBackground;
    private Image PlayerBulletGauge;
    private Image UltimateGauge;
    private Image DefaultUltIcon;
    public Image ActiveUltIcon;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = player.GetComponent<HealthPlayer>();

        PlayerUI = this.transform.GetChild(0).gameObject;

        PlayerHPGauge = PlayerUI.transform.GetChild(0).GetComponent<Image>();
        BulletGauge = PlayerUI.transform.GetChild(1).GetComponent<Image>();
        PlayerBulletGaugeBackground = this.transform.GetChild(1).gameObject;
        PlayerBulletGauge = PlayerBulletGaugeBackground.transform.GetChild(0).GetComponent<Image>();
        UltimateGauge = PlayerUI.transform.GetChild(3).GetComponent<Image>();
        DefaultUltIcon = PlayerUI.transform.GetChild(4).GetComponent<Image>();
        ActiveUltIcon = PlayerUI.transform.GetChild(5).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
    }

    private void GaugeManagement()
    {
        PlayerHPGauge.fillAmount = player.GetComponent<HealthPlayer>().CurrentHP() / 100;
        BulletGauge.fillAmount = player.GetComponent<HealthPlayer>().CurrentHP() / 100;
        PlayerBulletGauge.fillAmount = BulletGauge.fillAmount;
        UltimateGauge.fillAmount += Time.deltaTime / 10;
        PlayerBulletGaugeBackground.transform.position = Camera.main.WorldToScreenPoint(new Vector3(player.position.x + 0.7f, player.position.y + 0.7f, player.position.z));

        if(UltimateGauge.fillAmount == 1)
        {
            DefaultUltIcon.gameObject.SetActive(false);
            ActiveUltIcon.gameObject.SetActive(true);
        }
        else
        {
            DefaultUltIcon.gameObject.SetActive(true);
            ActiveUltIcon.gameObject.SetActive(false);
        }
    }
}
