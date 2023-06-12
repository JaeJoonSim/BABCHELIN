using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    public GameObject objectHitEffect;

    public void OnHitEffect()
    {
        GameObject hitEffect = Instantiate(objectHitEffect);
        hitEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
    }
}
