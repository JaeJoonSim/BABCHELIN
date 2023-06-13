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

    public List<string> GetGroupedDialogueNames(DialogueSystemGroupSO dialogueGroup, bool startingDialoguesOnly)
    {
        if (dialogueGroup == null)
        {
            Debug.LogError("DialogueSystemGroupSO argument is null");
            return new List<string>();
        }

        if (!DialogueGroups.ContainsKey(dialogueGroup))
        {
            Debug.LogError("Dialogue group not found in DialogueGroups");
            return new List<string>();
        }

        List<DialogueSystemDialogueSO> groupedDialogues = DialogueGroups[dialogueGroup];
        List<string> groupedDialogueNames = new List<string>();

        foreach (DialogueSystemDialogueSO groupedDialogue in groupedDialogues)
        {
            if (startingDialoguesOnly && !groupedDialogue.IsStartingDialogue)
                continue;

            groupedDialogueNames.Add(groupedDialogue.DialogueName);
        }

        return groupedDialogueNames;
    }


    public List<string> GetUnGroupedDialogueNames(bool startingDialoguesOnly)
    {
        List<string> ungroupedDialogueNames = new List<string>();

        foreach (DialogueSystemDialogueSO ungroupedDialogue in UngroupedDialogues)
        {
            if (startingDialoguesOnly && !ungroupedDialogue.IsStartingDialogue)
                continue;
            
            ungroupedDialogueNames.Add(ungroupedDialogue.DialogueName);
        }

        return ungroupedDialogueNames;
    }

    public DialogueSystemGroupSO GetGroupByIndex(int index)
    {
        if (index >= 0 && index < DialogueGroups.Count)
        {
            return new List<DialogueSystemGroupSO>(DialogueGroups.Keys)[index];
        }
        return null;
    }

    public DialogueSystemDialogueSO GetGroupedDialogue(DialogueSystemGroupSO dialogueGroup, bool startingDialoguesOnly)
    {
        if (DialogueGroups.ContainsKey(dialogueGroup))
        {
            List<DialogueSystemDialogueSO> groupedDialogues = DialogueGroups[dialogueGroup];

            foreach (DialogueSystemDialogueSO groupedDialogue in groupedDialogues)
            {
                if (startingDialoguesOnly && groupedDialogue.IsStartingDialogue)
                {
                    return groupedDialogue;
                }
                else if (!startingDialoguesOnly)
                {
                    return groupedDialogue;
                }
            }
        }
        return null;
    }
}
