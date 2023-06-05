using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] float destroyTime;
    private float time;
    private TMP_Text dmgText;
    private Color textColor;

    // Start is called before the first frame update
    void Start()
    {
        dmgText = GetComponent<TMP_Text>();
        textColor = new Color(0, 0, 0, 1);
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + (time / 500), transform.position.z - (time / 100));
        dmgText.color = textColor;
        textColor.a -= (Time.deltaTime / destroyTime);

        if (time > destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
