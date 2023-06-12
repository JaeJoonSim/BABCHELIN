using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class DialogueSystemDialogue : BaseMonoBehaviour
{
    [SerializeField] private DialogueSystemContainerSO dialogueContainer;
    [SerializeField] private DialogueSystemGroupSO dialogueGroup;
    [SerializeField] private DialogueSystemDialogueSO dialogue;

    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;

    [SerializeField] private int selectedDialogueGroupIndex;
    [SerializeField] private int selectedDialogueIndex;
    
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;
    [SerializeField] private Image backgroundPanel; 

    private DialogueSystemDialogueSO currentDialogue;

    private bool onTypeisRunning = false;

    private void OnEnable()
    {
        textUI.text = "";
        if (dialogueContainer != null)
        {
            dialogueGroup = dialogueContainer.GetGroupByIndex(selectedDialogueGroupIndex);
            if (dialogueGroup != null)
            {
                dialogue = dialogueContainer.GetGroupedDialogue(dialogueGroup, startingDialoguesOnly);
            }
        }

        currentDialogue = dialogue;

        StartCoroutine(OnType(0.1f, currentDialogue.Text));
        choiceButton1.onClick.AddListener(() => OnOptionChosen(0));
        choiceButton2.onClick.AddListener(() => OnOptionChosen(1));

        ChooseButton(currentDialogue, choiceButton1, 0);
        ChooseButton(currentDialogue, choiceButton2, 1);
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }


    private void Update()
    {
        SetDialogueTimeScale();
    }

    private void OnOptionChosen(int choiceIndex = 0)
    {
        if (!onTypeisRunning && gameObject.activeSelf)
        {
            textUI.text = "";
            DialogueSystemDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

            if (nextDialogue == null)
            {
                gameObject.SetActive(false);
                backgroundPanel.gameObject.SetActive(false);

                selectedDialogueGroupIndex++;
                if (selectedDialogueGroupIndex >= dialogueContainer.GetDialogueGroupNames().Count)
                {
                    selectedDialogueGroupIndex = 0;
                }

                DialogueSystemGroupSO nextGroup = dialogueContainer.DialogueGroups.Keys.ElementAt(selectedDialogueGroupIndex);
                dialogueGroup = nextGroup;

                Time.timeScale = 1;
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
            Time.timeScale = 1;
            choiceButton.gameObject.SetActive(false);
        }
    }

    IEnumerator OnType(float interval, string text)
    {
        onTypeisRunning = true;
        foreach (char item in text)
        {
            textUI.text += item;
            yield return new WaitForSecondsRealtime(interval);
        }
        onTypeisRunning = false;
    }

    private void SetDialogueTimeScale()
    {
        if (gameObject.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}