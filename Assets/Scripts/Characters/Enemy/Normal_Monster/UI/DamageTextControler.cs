using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextControler : MonoBehaviour
{
    public TMP_Text dmgText;
    public Canvas monsterUICanvas;
    public float textPosX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDamageText(float dmg)
    {
        dmgText.text = dmg.ToString();
        textPosX = Random.Range(-0.2f, 0.2f);
        dmgText.transform.position = new Vector3(dmgText.transform.position.x + textPosX, dmgText.transform.position.y, dmgText.transform.position.z);
        Instantiate(dmgText, monsterUICanvas.transform);
    }

    private void FadeoutDamageText()
    {

    }
}