using System;
using UnityEngine;

[Serializable]
public class DialogueSystemChoiceData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public DialogueSystemDialogueSO NextDialogue { get; set; }
}
