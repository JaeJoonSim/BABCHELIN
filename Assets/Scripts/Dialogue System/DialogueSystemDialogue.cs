using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystemDialogue : MonoBehaviour
{
    [SerializeField] private DialogueSystemContainerSO dialogueContainer;
    [SerializeField] private DialogueSystemGroupSO dialogueGroup;
    [SerializeField] private DialogueSystemDialogueSO dialogue;

    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;

    [SerializeField] private int selectedDialogueGroupIndex;
    [SerializeField] private int selectedDialogueIndex;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;

    private DialogueSystemDialogueSO currentDialogue;

    private bool onTypeisRunning = false;

    private void Start()
    {
        textUI.text = "";
        currentDialogue = dialogue;
        StartCoroutine(OnType(0.1f, currentDialogue.Text));
        choiceButton1.onClick.AddListener(() => OnOptionChosen(0));
        choiceButton2.onClick.AddListener(() => OnOptionChosen(1));

        ChooseButton(currentDialogue, choiceButton1, 0);
        ChooseButton(currentDialogue, choiceButton2, 1);
    }

    private void OnOptionChosen(int choiceIndex = 0)
    {
        if (!onTypeisRunning)
        {
            textUI.text = "";
            DialogueSystemDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

            if (nextDialogue == null)
            {
                dialoguePanel.SetActive(false);
                return;
            }

            currentDialogue = nextDialogue;

            ShowText();
            ChooseButton(currentDialogue, choiceButton1, 0);
            ChooseButton(currentDialogue, choiceButton2, 1);
        }
    }

    private void ShowText()
    {
        if (textUI != null)
            StartCoroutine(OnType(0.1f, currentDialogue.Text));
    }

    private void ChooseButton(DialogueSystemDialogueSO dialogue, Button choiceButton, int chooseIndex)
    {
        if (choiceButton != null && dialogue.Choices.Count > chooseIndex)
        {
            choiceButton.gameObject.SetActive(true);

            if (dialogue.Choices[chooseIndex].NextDialogue != null)
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = dialogue.Choices[chooseIndex].NextDialogue.DialogueName;
            else
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = "End";
        }
        else
        {
            choiceButton.gameObject.SetActive(false);
        }
    }

    IEnumerator OnType(float interval, string text)
    {
        onTypeisRunning = true;
        foreach (char item in text)
        {
            textUI.text += item;
            yield return new WaitForSeconds(interval);
        }
        onTypeisRunning = false;
    }
}