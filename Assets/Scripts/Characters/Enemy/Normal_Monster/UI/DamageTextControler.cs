using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextControler : MonoBehaviour
{
    public TMP_Text dmgText;
    public Canvas monsterUICanvas;

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
        Instantiate(dmgText, monsterUICanvas.transform);
    }

    private void FadeoutDamageText()
    {

    }
}