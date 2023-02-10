using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemNodeErrorData
{
    public DialogueSystemErrorData errorData { get; set; }
    public List<DialogueSystemNode> nodes { get; set; }

    public DialogueSystemNodeErrorData()
    {
        errorData = new DialogueSystemErrorData();
        nodes = new List<DialogueSystemNode>();
    }
}
