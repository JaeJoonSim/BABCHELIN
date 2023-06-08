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
    //private Image BulletGauge;
    //private Image UltimateGauge;
    private Image DefaultUltIcon;
    private Image ActiveUltIcon;

    public Image UIHealthGauge;
    public Image UIHealthBackGauge;
    public Slider UIHealthCutline;
    public Image UIBulletGauge;
    public Slider UIBulletCutline;

    public Transform BulletCanvas;
    private Image PlayerBulletGauge;

    public Image SkillIcon_1;
    public Image SkillIcon_2;
    public Image UIUltimateGauge;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = player.GetComponent<HealthPlayer>();

        PlayerUIBackground = this.transform.GetChild(0).transform;
        Debug.Log(PlayerUIBackground.gameObject.name);

        playerController = player.GetComponent<PlayerController>();

        PlayerBulletGauge = BulletCanvas.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();
    }

    private void GaugeManagement()
    {
        UIHealthGauge.fillAmount = playerHealth.CurrentHP() / playerHealth.MaxHP();
        UIHealthBackGauge.fillAmount = playerHealth.BackHP() / playerHealth.MaxHP();
        UIHealthCutline.value = playerHealth.CurrentHP() / playerHealth.MaxHP();
        UIBulletGauge.fillAmount = playerController.BulletGauge / 1000f;
        UIBulletCutline.value = playerController.BulletGauge / 1000f;
        PlayerBulletGauge.fillAmount = (UIBulletGauge.fillAmount / 5);


        //SkillIcon_1.fillAmount =  / playerController.BaseStatus.sk1CoolDown.value;
        UIUltimateGauge.fillAmount += Time.deltaTime / 10f;
    }
}