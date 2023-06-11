using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffObj : MonoBehaviour
{
    public int buffIdx;
    private void Start()
    {
        buffIdx = setIdx();
    }
    private int setIdx()
    {
        int tmp = Random.Range(1001, 1009);

        if (tmp == 1001 || tmp == 1003 || tmp == 1004 || tmp == 1009 )
            return setIdx();
        else
            return tmp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            absorb.Instance.Player.GetComponent<PlayerController>().addBuff(buffIdx, true);
            Destroy(gameObject);
        }
        
    }
}
