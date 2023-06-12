using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorb : BaseMonoBehaviour
{
    static private absorb instance;
    static public absorb Instance { get { return instance; } }

    public enum objectSize { small, medium, large };

    [Header("����� ����ð�")]

    public float absorbTimeSmall = 0.1f;

    public float absorbTimeMedium = 0.5f;

    public float absorbTimeLarge = 1f;

    [Header("����� źȯ ������")]

    public int addBulletSmall = 20;

    public int addBulletMedium = 50;

    public int addBulletLarge = 100;

    [Header("���� �� �����ð�")]
    public float absorbKeepTime = 1f;

    [Header("��� ��ġ")]
    private Transform player;

    public GameObject BulletEssence;
    public GameObject UltimateEssence;
    public Transform Player { 
        get        
        {
            return player;        
        } 
        set { player = value; } }
    [Header("��� �ӵ�")]
    public float speed = 10;

    public GameObject showAbsorbSmall;
    public GameObject showAbsorbMedium;
    public GameObject showAbsorbLarge;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void awake()
    {
        getPlayer();

        status stat = Player.GetComponent<PlayerController>().TotalStatus;
        absorbTimeSmall = stat.absorbSpdSmall.value;
        absorbTimeMedium = stat.absorbSpdMedium.value;
        absorbTimeLarge = stat.absorbSpdLarge.value;

        addBulletSmall = stat.absorbChargeSmall.value;
        addBulletMedium = stat.absorbChargeMedium.value;
        addBulletLarge = stat.absorbChargeLarge.value;
    }

    private void Update()
    {
        if (player == null)
        {
            getPlayer();
        }
    }

    private void getPlayer()
    {
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.layer == LayerMask.NameToLayer("Player"))
                {
                    Player = player.transform;
                    break;
                }
            }
        }
    }
}
