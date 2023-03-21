using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefrigeratorScript : MonoBehaviour
{
    [Tooltip("≥√¿Â∞Ì")]
    [SerializeField]
    private Inventory refrigerator;
    public Inventory Refrigerator { get { return refrigerator; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PreservationStep();
    }

    void PreservationStep()
    {
        for (int i = 0; i < refrigerator.Items.Items.Length; i++)
        {
            if (4800 < refrigerator.Items.Items[i].Item.FRESHNESS && refrigerator.Items.Items[i].Item.FRESHNESS <= 7200)
            {

            }
            else if (2400 < refrigerator.Items.Items[i].Item.FRESHNESS && refrigerator.Items.Items[i].Item.FRESHNESS <= 4800)
            {

            }
            else
            {

            }

            if (refrigerator.Items.Items[i].Item.FRESHNESS <= 0)
            {
                refrigerator.Items.Items[i].Item.FRESHNESS = 0;
            }

            if (refrigerator.Items.Items[i].Amount >= 1)     //
            {
                refrigerator.Items.Items[i].Item.FRESHNESS -= Time.deltaTime;  //
            }

        }
    }
}