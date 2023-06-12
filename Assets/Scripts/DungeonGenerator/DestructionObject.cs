using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionObject : MonoBehaviour
{
    public ParticleSystem DestructionEffet;

    public float zPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttack"))
        {
            BackGroundSouund.Instance.PlaySound("objectDestroy");
            Instantiate(DestructionEffet, transform.position + new Vector3(0,0, zPos) , Quaternion.identity);
        }
    }

}
