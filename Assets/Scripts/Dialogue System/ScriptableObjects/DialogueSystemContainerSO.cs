using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemContainerSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public SerializableDictionary<DialogueSystemGroupSO, List<DialogueSystemDialogueSO>> DialogueGroups { get; set; }
    [field: SerializeField] public List<DialogueSystemDialogueSO> UngroupedDialogues { get; set; }

    public void Initialize(string fileName)
    {
        FileName = fileName;
        DialogueGroups = new SerializableDictionary<DialogueSystemGroupSO, List<DialogueSystemDialogueSO>>();
        UngroupedDialogues = new List<DialogueSystemDialogueSO>();
    }

    public List<string> GetDialogueGroupNames()
    {
        List<string> dialogueGroupNames = new List<string>();
        foreach (DialogueSystemGroupSO dialogueGroup in DialogueGroups.Keys)
        {
            dialogueGroupNames.Add(dialogueGroup.GroupName);
        }
        return dialogueGroupNames;
    }

    public List<string> GetGroupedDialogueNames(DialogueSystemGroupSO dialogueGroup)
    {
        List<DialogueSystemDialogueSO> groupedDialogues = DialogueGroups[dialogueGroup];
        List<string> groupedDialogueNames = new List<string>();
        foreach (DialogueSystemDialogueSO dialogue in groupedDialogues)
        {
            groupedDialogueNames.Add(dialogue.DialogueName);
        }
        return groupedDialogueNames;
    }

    public List<string> GetUngroupedDialogueNames()
    {
        List<string> ungroupedDialogueNames = new List<string>();
        foreach (DialogueSystemDialogueSO ungroupedDialogue in UngroupedDialogues)
        {
            ungroupedDialogueNames.Add(ungroupedDialogue.DialogueName);
        }
        return ungroupedDialogueNames;
    }
}
