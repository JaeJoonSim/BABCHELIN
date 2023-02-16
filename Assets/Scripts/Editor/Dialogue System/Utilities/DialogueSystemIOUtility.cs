using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#if UNITY_EDITOR
public static class DialogueSystemIOUtility
{
    private static DialogueSystemGraphView graphView;

    private static string graphFileName;
    private static string containerFolderPath;

    private static List<DialogueSystemGroup> groups;
    private static List<DialogueSystemNode> nodes;

    private static Dictionary<string, DialogueSystemGroupSO> createdDialogueGroups;
    private static Dictionary<string, DialogueSystemDialogueSO> createDialogues;
    
    private static Dictionary<string, DialogueSystemGroup> loadedGroups;
    private static Dictionary<string, DialogueSystemNode> loadedNodes;

    public static void Initialize(DialogueSystemGraphView dialogueSystemGraphView, string fileName)
    {
        graphView = dialogueSystemGraphView;
        graphFileName = fileName;
        containerFolderPath = $"Assets/Dialogue Data/Dialogues/{graphFileName}";

        groups = new List<DialogueSystemGroup>();
        nodes = new List<DialogueSystemNode>();

        createdDialogueGroups = new Dictionary<string, DialogueSystemGroupSO>();
        createDialogues = new Dictionary<string, DialogueSystemDialogueSO>();

        loadedGroups = new Dictionary<string, DialogueSystemGroup>();
        loadedNodes = new Dictionary<string, DialogueSystemNode>();
    }

    #region Save Methods
    public static void Save()
    {
        CreatestaticFolder();
        GetElementsFromGraphView();

        DialogueSystemGraphSaveData graphData = CreateAsset<DialogueSystemGraphSaveData>("Assets/Scripts/Editor/Dialogue Data/Graphs", $"{graphFileName}Graph");
        graphData.Initialize(graphFileName);

        DialogueSystemContainerSO dialogueContainer = CreateAsset<DialogueSystemContainerSO>(containerFolderPath, graphFileName);
        dialogueContainer.Initialize(graphFileName);

        SaveGroups(graphData, dialogueContainer);
        SaveNodes(graphData, dialogueContainer);

        SaveAsset(graphData);
        SaveAsset(dialogueContainer);
    }

    #region Groups
    private static void SaveGroups(DialogueSystemGraphSaveData graphData, DialogueSystemContainerSO dialogueSystemContainer)
    {
        List<string> groupNames = new List<string>();

        foreach (DialogueSystemGroup group in groups)
        {
            SaveGroupToGraph(group, graphData);
            SaveGroupToScriptableObject(group, dialogueSystemContainer);
            groupNames.Add(group.title);
        }

        UpdateOldGroups(groupNames, graphData);
    }

    private static void SaveGroupToGraph(DialogueSystemGroup group, DialogueSystemGraphSaveData graphData)
    {
        DialogueSystemGroupSaveData groupData = new DialogueSystemGroupSaveData()
        {
            ID = group.ID,
            Name = group.title,
            Position = group.GetPosition().position
        };

        graphData.Groups.Add(groupData);
    }

    private static void SaveGroupToScriptableObject(DialogueSystemGroup group, DialogueSystemContainerSO dialogueContainer)
    {
        string groupName = group.title;

        CreateFolder($"{containerFolderPath}/Groups", groupName);
        CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

        DialogueSystemGroupSO dialogueGroup = CreateAsset<DialogueSystemGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);
        dialogueGroup.Initialize(groupName);
        createdDialogueGroups.Add(group.ID, dialogueGroup);
        dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DialogueSystemDialogueSO>());

        SaveAsset(dialogueGroup);
    }

    private static void UpdateOldGroups(List<string> currentGroupNames, DialogueSystemGraphSaveData graphData)
    {
        if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
        {
            List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

            foreach (string groupToRemove in groupsToRemove)
            {
                RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }
    }
    #endregion

    #region Nodes
    private static void SaveNodes(DialogueSystemGraphSaveData graphData, DialogueSystemContainerSO dialogueContainer)
    {
        SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
        List<string> ungroupedNodeNames = new List<string>();

        foreach (DialogueSystemNode node in nodes)
        {
            SaveNodeToGraph(node, graphData);
            SaveNodeToScriptableObject(node, dialogueContainer);

            if (node.Group != null)
            {
                groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                continue;
            }

            ungroupedNodeNames.Add(node.DialogueName);
        }

        UpdateDialogueChoicesConnections();
        UpdateOldGroupedNodes(groupedNodeNames, graphData);
        UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
    }

    private static void SaveNodeToGraph(DialogueSystemNode node, DialogueSystemGraphSaveData graphData)
    {
        List<DialogueSystemChoiceSaveData> choices = CloneNodeChoices(node.Choices);

        DialogueSystemNodeSaveData nodeData = new DialogueSystemNodeSaveData()
        {
            ID = node.ID,
            Name = node.DialogueName,
            Choices = choices,
            Text = node.Text,
            GroupID = node.Group?.ID,
            DialogueType = node.DialogueType,
            Position = node.GetPosition().position
        };

        graphData.Nodes.Add(nodeData);
    }

    private static void SaveNodeToScriptableObject(DialogueSystemNode node, DialogueSystemContainerSO dialogueContainer)
    {
        DialogueSystemDialogueSO dialogue;

        if (node.Group != null)
        {
            dialogue = CreateAsset<DialogueSystemDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
            dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
        }
        else
        {
            dialogue = CreateAsset<DialogueSystemDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
            dialogueContainer.UngroupedDialogues.Add(dialogue);
        }

        dialogue.Initialize(
            node.DialogueName,
            node.Text,
            ConvertNodeChoicesToDialogueChoices(node.Choices),
            node.DialogueType,
            node.IsStartingNode()
            );

        createDialogues.Add(node.ID, dialogue);

        SaveAsset(dialogue);
    }

    private static List<DialogueSystemChoiceData> ConvertNodeChoicesToDialogueChoices(List<DialogueSystemChoiceSaveData> nodeChoices)
    {
        List<DialogueSystemChoiceData> dialougeChoices = new List<DialogueSystemChoiceData>();

        foreach (DialogueSystemChoiceSaveData nodeChoice in nodeChoices)
        {
            DialogueSystemChoiceData choiceData = new DialogueSystemChoiceData()
            {
                Text = nodeChoice.Text
            };

            dialougeChoices.Add(choiceData);
        }

        return dialougeChoices;
    }

    private static void UpdateDialogueChoicesConnections()
    {
        foreach (DialogueSystemNode node in nodes)
        {
            DialogueSystemDialogueSO dialogue = createDialogues[node.ID];

            for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
            {
                DialogueSystemChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                if (string.IsNullOrEmpty(nodeChoice.NodeID))
                {
                    continue;
                }

                dialogue.Choices[choiceIndex].NextDialogue = createDialogues[nodeChoice.NodeID];
                SaveAsset(dialogue);
            }
        }
    }

    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentgroupedNodeNames, DialogueSystemGraphSaveData graphData)
    {
        if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
        {
            foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
            {
                List<string> nodesToRemove = new List<string>();

                if (currentgroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                {
                    nodesToRemove = oldGroupedNode.Value.Except(currentgroupedNodeNames[oldGroupedNode.Key]).ToList();
                }

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                }
            }
        }

        graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentgroupedNodeNames);
    }

    private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DialogueSystemGraphSaveData graphData)
    {
        if (graphData.OldUnGroupedNodeNames != null && graphData.OldUnGroupedNodeNames.Count != 0)
        {
            List<string> nodesToRemove = graphData.OldUnGroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
            }
        }

        graphData.OldUnGroupedNodeNames = new List<string>(currentUngroupedNodeNames);
    }
    #endregion
    #endregion

    #region Load Methods
    public static void Load()
    {
        DialogueSystemGraphSaveData graphData = LoadAsset<DialogueSystemGraphSaveData>("Assets/Scripts/Editor/Dialogue Data/Graphs", graphFileName);

        if (graphData == null)
        {
            EditorUtility.DisplayDialog(
                "Couldn't load the file!",
                "The file at the following path could be found:\n\n" + 
                $"Assets/Scripts/Editor/Dialogue Data/Graphs/{graphFileName}\n\n" +
                "Make sure you choose the right file and it's placed at the floder path mentioned above.",
                "Thanks!"
                );
            return;
        }

        DialogueSystemEditorWindow.UpdateFileName(graphData.FileName);
        LoadGroups(graphData.Groups);
        LoadNodes(graphData.Nodes);
        LoadNodesConnections();
    }

    private static void LoadGroups(List<DialogueSystemGroupSaveData> groups)
    {
        foreach (DialogueSystemGroupSaveData groupData in groups)
        {
            DialogueSystemGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);
            group.ID = groupData.ID;
            loadedGroups.Add(group.ID, group);
        }
    }

    private static void LoadNodes(List<DialogueSystemNodeSaveData> nodes)
    {
        foreach(DialogueSystemNodeSaveData nodeData in nodes)
        {
            List<DialogueSystemChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
            DialogueSystemNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

            node.ID = nodeData.ID;
            node.Choices = choices;
            node.Text = nodeData.Text;

            node.Draw();
            graphView.AddElement(node);
            loadedNodes.Add(node.ID, node);

            if (string.IsNullOrEmpty(nodeData.GroupID))
            {
                continue;
            }

            DialogueSystemGroup group = loadedGroups[nodeData.GroupID];
            node.Group = group;
            group.AddElement(node);
        }
    }

    private static void LoadNodesConnections()
    {
        foreach(KeyValuePair<string, DialogueSystemNode> loadedNode in loadedNodes)
        {
            foreach(Port choicePort in loadedNode.Value.outputContainer.Children())
            {
                DialogueSystemChoiceSaveData choiceData = (DialogueSystemChoiceSaveData)choicePort.userData;

                if (string.IsNullOrEmpty(choiceData.NodeID))
                    continue;

                DialogueSystemNode nextNode = loadedNodes[choiceData.NodeID];
                Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
                Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                graphView.AddElement(edge);
                loadedNode.Value.RefreshPorts();
            }
        }
    }
    #endregion

    #region Creation Methods
    private static void CreatestaticFolder()
    {
        CreateFolder("Assets/Scripts/Editor", "Dialogue Data");
        CreateFolder("Assets/Scripts/Editor/Dialogue Data", "Graphs");
        CreateFolder("Assets", "Dialogue Data");
        CreateFolder("Assets/Dialogue Data", "Dialogues");
        CreateFolder("Assets/Dialogue Data/Dialogues", graphFileName);
        CreateFolder(containerFolderPath, "Global");
        CreateFolder(containerFolderPath, "Groups");
        CreateFolder($"{containerFolderPath}/Global", "Dialogues");
    }
    #endregion

    #region Fetch Methods
    private static void GetElementsFromGraphView()
    {
        Type groupType = typeof(DialogueSystemGroup);

        graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is DialogueSystemNode node)
            {
                nodes.Add(node);
                return;
            }

            if (graphElement.GetType() == groupType)
            {
                DialogueSystemGroup group = (DialogueSystemGroup)graphElement;
                groups.Add(group);
                return;
            }
        });
    }
    #endregion

    #region Utility Methods
    public static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }
        AssetDatabase.CreateFolder(path, folderName);
    }

    public static void RemoveFolder(string fullPath)
    {
        FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
        FileUtil.DeleteFileOrDirectory($"{fullPath}/");
    }

    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";
        T asset = LoadAsset<T>(path, fullPath);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, fullPath);
        }
        return asset;
    }

    public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";
        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }

    public static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }

    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static List<DialogueSystemChoiceSaveData> CloneNodeChoices(List<DialogueSystemChoiceSaveData> nodeChoices)
    {
        List<DialogueSystemChoiceSaveData> choices = new List<DialogueSystemChoiceSaveData>();

        foreach (DialogueSystemChoiceSaveData choice in nodeChoices)
        {
            DialogueSystemChoiceSaveData choiceData = new DialogueSystemChoiceSaveData()
            {
                Text = choice.Text,
                NodeID = choice.NodeID
            };

            choices.Add(choiceData);
        }

        return choices;
    }
    #endregion
}
#endif