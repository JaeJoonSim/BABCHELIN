using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemDialogue : MonoBehaviour
{
    [SerializeField] private DialogueSystemContainerSO dialogueContainer;
    [SerializeField] private DialogueSystemGroupSO dialogueGroup;
    [SerializeField] private DialogueSystemDialogueSO dialogue;

    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;

    [SerializeField] private int selectedDialogueGroupIndex;
    [SerializeField] private int selectedDialogueIndex;
}