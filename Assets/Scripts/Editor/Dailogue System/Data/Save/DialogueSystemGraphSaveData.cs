using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemGraphSaveData : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public List<DialogueSystemGroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<DialogueSystemNodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<string> OldGroupNames { get; set; }
    [field: SerializeField] public List<string> OldUnGroupedNodeNames { get; set; }
    [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }
    
    public void Initialize(string fileName)
    {
        FileName = fileName;

        Groups = new List<DialogueSystemGroupSaveData>();
        Nodes = new List<DialogueSystemNodeSaveData>();
    }
}
