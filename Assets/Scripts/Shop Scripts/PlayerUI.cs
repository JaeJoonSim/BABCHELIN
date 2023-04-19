using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Transform player;
    private Health playerHealth;
    private PlayerController playerController;

    private Transform PlayerUIBackground;
    private Image PlayerHPGauge;
    private Image BulletGauge;
    private Image UltimateGauge;
    private Image DefaultUltIcon;
    private Image ActiveUltIcon;

    public Transform BulletCanvas;
    private Image PlayerBulletGauge;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = player.GetComponent<HealthPlayer>();

        PlayerUIBackground = this.transform.GetChild(0).transform;

        PlayerHPGauge = PlayerUIBackground.GetChild(0).GetComponent<Image>();
        BulletGauge = PlayerUIBackground.GetChild(1).GetComponent<Image>();
        UltimateGauge = PlayerUIBackground.GetChild(3).GetComponent<Image>();
        DefaultUltIcon = PlayerUIBackground.GetChild(4).GetComponent<Image>();
        ActiveUltIcon = PlayerUIBackground.GetChild(5).GetComponent<Image>();
        playerController = GetComponentInParent<PlayerController>();

        PlayerBulletGauge = BulletCanvas.GetChild(0).transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
    }

    private void GaugeManagement()
    {
        PlayerHPGauge.fillAmount = playerHealth.CurrentHP() / playerHealth.MaxHP();
        BulletGauge.fillAmount = playerController.BulletGauge / 1000f;
        UltimateGauge.fillAmount += Time.deltaTime / 10f;

        PlayerBulletGauge.fillAmount = BulletGauge.fillAmount;


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
