using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenSetting : MonoBehaviour
{
    public TMP_Dropdown resolution;
    public TMP_Dropdown screenType;
    public bool isFullscreen = true;


    private void Update()
    {
        screenTypeSetting();
        resolutionSetting();
    }

    private void resolutionSetting()
    {
        switch(resolution.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, isFullscreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, isFullscreen);
                break;
            case 2:
                Screen.SetResolution(2560, 1440, isFullscreen);
                break;
        }
    }

    private void screenTypeSetting()
    {
        switch (screenType.value)
        {
            case 0:
                isFullscreen = true;
                break;

            case 1:
                isFullscreen = false;
                break;
        }
    }
}
