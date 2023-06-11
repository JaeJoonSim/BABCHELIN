using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : UDRLBullet
{
    // Start is called before the first frame update

    [SerializeField] float blastRange;

    [SerializeField] Transform target;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

}
