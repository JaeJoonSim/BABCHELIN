using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenSetting : MonoBehaviour
{
    [SerializeField]private int resolution;
    public TMP_Text resolutionText;
    public Button resolutionLeftButton;
    public Button resolutionRightButton;
    [SerializeField] private int screenType;
    public TMP_Text screenTypeText;
    public Button screenTypeLeftButton;
    public Button screenTypeRightButton;
    public bool isFullscreen = true;


    private void Update()
    {
        resolutionSetting();
        screenTypeSetting();
    }

    private void resolutionSetting()
    {
        switch(resolution)
        {
            case 0:
                resolutionText.text = "1280 * 720";
                Screen.SetResolution(1280, 720, isFullscreen);
                break;
            case 1:
                resolutionText.text = "1920 * 1080";
                Screen.SetResolution(1920, 1080, isFullscreen);
                break;
            case 2:
                resolutionText.text = "2560 * 1440";
                Screen.SetResolution(2560, 1440, isFullscreen);
                break;
        }
    }

    private void screenTypeSetting()
    {
        switch (screenType)
        {
            case 0:
                screenTypeText.text = "Full Screen";
                isFullscreen = true;
                break;

            case 1:
                screenTypeText.text = "Windowed mode";
                isFullscreen = false;
                break;
        }
    }

    public void resolutionLeftButtonClick()
    {
        if(0 < resolution)
            resolution--;
    }

    public void resolutionRightButtonClick()
    {
        if (resolution < 2)
            resolution++;
    }

    public void screenTypeLeftButtonClick()
    {
        if(0 < screenType)
            screenType--;
    }

    public void screenTypeRightButtonClick()
    {
        if (screenType < 1)
            screenType++;
    }
}
