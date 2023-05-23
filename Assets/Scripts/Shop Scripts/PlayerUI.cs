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
    public Image UIBulletGauge;
    public Image UIUltimateGauge;
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
        Debug.Log(PlayerUIBackground.gameObject.name);

        //DefaultUltIcon = PlayerUIBackground.GetChild(4).GetComponent<Image>();
        //ActiveUltIcon = PlayerUIBackground.GetChild(5).GetComponent<Image>();
        playerController = GetComponentInParent<PlayerController>();

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
        UIBulletGauge.fillAmount = playerController.BulletGauge / 1000f;
        UIUltimateGauge.fillAmount += Time.deltaTime / 10f;

        PlayerBulletGauge.fillAmount = (UIBulletGauge.fillAmount / 5);


        //if (UIUltimateGauge.fillAmount == 1)
        //{
        //    DefaultUltIcon.gameObject.SetActive(false);
        //    ActiveUltIcon.gameObject.SetActive(true);
        //}
        //else
        //{
        //    DefaultUltIcon.gameObject.SetActive(true);
        //    ActiveUltIcon.gameObject.SetActive(false);
        //}
    }
}