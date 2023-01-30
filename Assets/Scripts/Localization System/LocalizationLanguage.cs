using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationLanguage : MonoBehaviour
{
    public void Awake()
    {
        LocalizationManager.Read();

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                LocalizationManager.Language = "Korean";
                break;
            default:
                LocalizationManager.Language = "English";
                break;
        }
    }

    public void SetLocalization(string localization)
    {
        LocalizationManager.Language = localization;
    }
}
