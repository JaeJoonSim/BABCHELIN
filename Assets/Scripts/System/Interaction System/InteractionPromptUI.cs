using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("UI패널")]
    private GameObject uiPanel;
    public GameObject UiPanel
    {
        get { return uiPanel; }
        set { uiPanel = value; }
    }

    [SerializeField]
    [Tooltip("UI텍스트 Key")]
    private LocalizedText promptText;
    public LocalizedText PromptText
    {
        get { return promptText; }
        set { promptText = value; }
    }

    private Camera mainCam;

    [SerializeField]
    [Tooltip("UI노출 유무")]
    private bool isDisplayed = false;
    public bool IsDisplayed
    {
        get { return isDisplayed; }
        set { isDisplayed = value; }
    }

    private void Start()
    {
        mainCam = Camera.main;
        uiPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetUp(string pText)
    {
        promptText.LocalizationKey = pText;
        uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        uiPanel.SetActive(false);
        IsDisplayed = false;
    }
}
