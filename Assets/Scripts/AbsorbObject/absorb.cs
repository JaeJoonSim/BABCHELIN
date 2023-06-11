using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class absorb : BaseMonoBehaviour
{
    static private absorb instance;
    static public absorb Instance { get { return instance; } }

    public enum objectSize { small, medium, large };

    [Header("사이즈별 흡수시간")]

    public float absorbTimeSmall = 0.1f;

    public float absorbTimeMedium = 0.5f;

    public float absorbTimeLarge = 1f;

    [Header("사이즈별 탄환 충전량")]

    public int addBulletSmall = 20;

    public int addBulletMedium = 50;

    public int addBulletLarge = 100;

    [Header("범위 밖 유지시간")]
    public float absorbKeepTime = 1f;

    [Header("흡수 위치")]
    private Transform player;

    public GameObject BulletEssence;
    public GameObject UltimateEssence;
    public Transform Player { get { return player; } set { player = value; } }
    [Header("흡수 속도")]
    public float speed = 10;

    public GameObject showAbsorb;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Player == null)
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
