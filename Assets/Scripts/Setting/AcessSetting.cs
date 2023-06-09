using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AcessSetting : MonoBehaviour
{
    public static float cameraShakeMin;
    public static float cameraShakeMax;
    public Slider CameraShakeSlider;
    [SerializeField] private int textSize;
    public TMP_Text textSizeText;
    [SerializeField] private int textSpeed;
    public TMP_Text textSpeedText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraShakeSetting();
        textSizeSetting();
        textSpeedSetting();
    }

    private void cameraShakeSetting()
    {
        cameraShakeMin = CameraShakeSlider.value * 0.6f;
        cameraShakeMax = CameraShakeSlider.value * 0.8f;
    }

    private void textSizeSetting()
    {
        switch (textSize)
        {
            case 0:
                textSizeText.text = "Small";
                break;
            case 1:
                textSizeText.text = "Medium";
                break;
            case 2:
                textSizeText.text = "Large";
                break;
        }
    }

    private void textSpeedSetting()
    {
        switch (textSpeed)
        {
            case 0:
                textSpeedText.text = "Slow";
                break;
            case 1:
                textSpeedText.text = "Middle";
                break;
            case 2:
                textSpeedText.text = "Fast";
                break;
        }
    }

    public void textSizeLeftButtonClick()
    {
        if (0 < textSize)
            textSize--;
    }

    public void textSizeRightButtonClick()
    {
        if (textSize < 2)
            textSize++;
    }

    public void textSpeedLeftButtonClick()
    {
        if (0 < textSpeed)
            textSpeed--;
    }

    public void textSpeedRightButtonClick()
    {
        if (textSpeed < 2)
            textSpeed++;
    }
}
