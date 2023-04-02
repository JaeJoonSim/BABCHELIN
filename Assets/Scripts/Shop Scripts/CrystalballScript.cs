using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalballScript : MonoBehaviour
{
    public GameObject DungeonSelectUI;
    public GameObject WeaponUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputDungeonButton()
    {
        this.gameObject.SetActive(false);
        DungeonSelectUI.SetActive(true);
    }

    public void InputWeaponButton()
    {
        this.gameObject.SetActive(false);
        WeaponUI.SetActive(true);
    }
}
