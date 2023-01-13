using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryPopupUI : MonoBehaviour
{
    [Header("Confirmation Popup")]
    [SerializeField] private GameObject confirmationPopupObject;
    [SerializeField] private TextMeshProUGUI confirmationItemNameText;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmationOkButton;
    [SerializeField] private Button confirmationCancelButton;

    [Header("Amount Input Popup")]
    [SerializeField] private GameObject amountInputPopupObject;
    [SerializeField] private TextMeshProUGUI amountInputItemNameText;
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private Button amountPlusButton;
    [SerializeField] private Button amountMinusButton;
    [SerializeField] private Button amountInputOkButton;
    [SerializeField] private Button amountInputCancelButton;

    private event Action OnConfirmationOK;
    private event Action<int> OnAmountInputOK;

    private int maxAmount;

    private void Awake()
    {
        InitUIEvents();
        HidePanel();
        HideConfirmationPopup();
        HideAmountInputPopup();
    }

    private void Update()
    {
        if(confirmationPopupObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                confirmationOkButton.onClick?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                confirmationCancelButton.onClick?.Invoke();
            }
        }
        else if (amountInputPopupObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                amountInputOkButton.onClick?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                amountInputCancelButton.onClick?.Invoke();
            }
        }
    }

    public void OpenConfirmationPopup(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopup(itemName);
        SetConfirmationOKEvent(okCallback);
    }
    
    /// <summary> 수량 입력 팝업 띄우기 </summary>
    public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
    {
        maxAmount = currentAmount - 1;
        amountInputField.text = "1";

        ShowPanel();
        ShowAmountInputPopup(itemName);
        SetAmountInputOKEvent(okCallback);
    }

    private void InitUIEvents()
    {
        confirmationOkButton.onClick.AddListener(HidePanel);
        confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
        confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        confirmationCancelButton.onClick.AddListener(HidePanel);
        confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);

        amountInputOkButton.onClick.AddListener(HidePanel);
        amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
        amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(amountInputField.text)));

        amountInputCancelButton.onClick.AddListener(HidePanel);
        amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

        amountMinusButton.onClick.AddListener(() =>
        {
            int.TryParse(amountInputField.text, out int amount);
            if (amount > 1)
            {
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                if (nextAmount < 1)
                    nextAmount = 1;
                amountInputField.text = nextAmount.ToString();
            }
        });

        amountPlusButton.onClick.AddListener(() =>
        {
            int.TryParse(amountInputField.text, out int amount);
            if (amount < maxAmount)
            {
                // Shift 누르면 10씩 증가
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                if (nextAmount > maxAmount)
                    nextAmount = maxAmount;
                amountInputField.text = nextAmount.ToString();
            }
        });

        amountInputField.onValueChanged.AddListener(str =>
        {
            int.TryParse(str, out int amount);
            bool flag = false;

            if (amount < 1)
            {
                flag = true;
                amount = 1;
            }
            else if (amount > maxAmount)
            {
                flag = true;
                amount = maxAmount;
            }

            if (flag)
                amountInputField.text = amount.ToString();
        });
    }

    private void ShowPanel() => gameObject.SetActive(true);
    private void HidePanel() => gameObject.SetActive(false);

    private void ShowConfirmationPopup(string itemName)
    {
        confirmationItemNameText.text = itemName;
        confirmationPopupObject.SetActive(true);
    }

    private void HideConfirmationPopup() => confirmationPopupObject.SetActive(false);

    private void ShowAmountInputPopup(string itemName)
    {
        amountInputItemNameText.text = itemName;
        amountInputPopupObject.SetActive(true);
    }

    private void HideAmountInputPopup() => amountInputPopupObject.SetActive(false);

    private void SetConfirmationOKEvent(Action handler) => OnConfirmationOK = handler;
    private void SetAmountInputOKEvent(Action<int> handler) => OnAmountInputOK = handler;
}
