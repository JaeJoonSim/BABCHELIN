using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystemDialogue : MonoBehaviour
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

    private DialogueSystemDialogueSO currentDialogue;

    private void Start()
    {
        currentDialogue = dialogue;
        textUI.text = currentDialogue.Text;
        choiceButton1.onClick.AddListener(() => OnOptionChosen(0));
        choiceButton2.onClick.AddListener(() => OnOptionChosen(1));
        
        ChooseButton(choiceButton1, 0);
        ChooseButton(choiceButton2, 1);
    }

    private void ShowText()
    {
        if (textUI != null)
            textUI.text = currentDialogue.Text;
    }

    private void OnOptionChosen(int choiceIndex = 0)
    {
        DialogueSystemDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

        if (nextDialogue == null)
        {
            return;
        }

        currentDialogue = nextDialogue;

        ShowText();
        ChooseButton(choiceButton1, 0);
        ChooseButton(choiceButton2, 1);
    }

    private void ChooseButton(Button choiceButton, int chooseIndex)
    {
        if (choiceButton != null && currentDialogue.Choices.Count > chooseIndex)
        {
            choiceButton.gameObject.SetActive(true);
            if (currentDialogue.Choices[chooseIndex].NextDialogue != null)
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.Choices[chooseIndex].NextDialogue.DialogueName;
        }
        else
        {
            choiceButton.gameObject.SetActive(false);
        }
    }
}