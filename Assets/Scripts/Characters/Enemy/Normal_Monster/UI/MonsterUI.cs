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

    // Start is called before the first frame update
    void Start()
    {
        monsterHealth = monster.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeManagement();

        if (monster.localScale.x < 0)
        {
            UIHealthGauge.gameObject.transform.localScale = new Vector3(-0.004f, 0.004f, 0.004f);
        }
        else
        {
            UIHealthGauge.gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
        }
    }

    private void GaugeManagement()
    {
        UIHealthGauge.fillAmount = monsterHealth.CurrentHP() / monsterHealth.MaxHP();
    }
}