using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemChoiceData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public DialogueSystemDialogueSO NextDialogue { get; set; }
}
