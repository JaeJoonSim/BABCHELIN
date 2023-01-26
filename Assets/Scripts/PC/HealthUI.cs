using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Tooltip("리필 속도")]
    [SerializeField]
    private float refillSpeed = 0.5f;

    [Space, Header("Debug")]
    [SerializeField]
    private bool refilling;

    private Slider healthSlider;
    private TextMeshProUGUI healthText;

    private void Start()
    {
        healthSlider = GetComponent<Slider>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        healthText.text = (healthSlider.value * 10).ToString("n0");

        if (refilling)
        {
            healthSlider.value = healthSlider.value < 1 ? Mathf.MoveTowards(healthSlider.value, healthSlider.maxValue, refillSpeed * Time.deltaTime) : healthSlider.value;
            if (healthSlider.value >= 1)
                refilling = false;
        }
    }
}
