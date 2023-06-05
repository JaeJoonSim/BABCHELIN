using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] float destroyTime;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (time / 100));

        if (time > destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
