using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerrybirdBulletScript : BulletScript
{

    public GameObject AreaDotObject;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(transform.position.z >= -0.03333164f)
        {
            AreaDotObject.transform.position = transform.position;
            GameObject areadotobject = Instantiate(AreaDotObject);

            Destroy(gameObject);
        }
    }
}
