using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Group = UnityEditor.Experimental.GraphView.Group;

#if UNITY_EDITOR
public class DialogueSystemGraphView : GraphView
{
    private DialogueSystemEditorWindow editorWindow;
    private DialogueSystemSearchWindow searchWindow;

    private SerializableDictionary<string, DialogueSystemNodeErrorData> ungroupedNodes;
    private SerializableDictionary<string, DialogueSystemGroupErrorData> groups;
    private SerializableDictionary<Group, SerializableDictionary<string, DialogueSystemNodeErrorData>> groupedNodes;

    private int repeatedNamesAmount;
    
    public int RepeatedNameAmount
    {
        get { return repeatedNamesAmount; }
        set 
        { 
            repeatedNamesAmount = value;

            if (repeatedNamesAmount == 0)
            {
                editorWindow.EnableSaving();
            }
            if(repeatedNamesAmount == 1)
            {
                editorWindow.DisableSaving();
            }
        }
    }

    public DialogueSystemGraphView(DialogueSystemEditorWindow editorWindow)
    {
        this.editorWindow = editorWindow;

        ungroupedNodes = new SerializableDictionary<string, DialogueSystemNodeErrorData>();
        groups = new SerializableDictionary<string, DialogueSystemGroupErrorData>();
        groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogueSystemNodeErrorData>>();

        // 순서 중요
        AddManipulators();
        AddSearchWindow();
        AddGridBackground();

        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        OnGraphViewChange();

        AddStyles();
    }

    #region Override Methods
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort == port)
                return;
            if (startPort.node == port.node)
                return;
            if (startPort.direction == port.direction)
                return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
    #endregion

    #region Manipilators
    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // 순서 중요
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueSystemType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueSystemType.MultipleChoice));

        this.AddManipulator(CreateGroupContextualMenu("Add Group"));
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueSystemType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

        return contextualMenuManipulator;
    }

    private IManipulator CreateGroupContextualMenu(string actionTitle)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => CreateGroup("Dialogue Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

        return contextualMenuManipulator;
    }
    #endregion

    #region Elements Creation
    public DialogueSystemGroup CreateGroup(string title, Vector2 localMousePosition)
    {
        DialogueSystemGroup group = new DialogueSystemGroup(title, localMousePosition);

        AddGroup(group);
        AddElement(group);

        foreach (GraphElement selectedElement in selection)
        {
            if (!(selectedElement is DialogueSystemNode))
            {
                continue;
            }

            DialogueSystemNode node = (DialogueSystemNode)selectedElement;
            group.AddElement(node);
        }

        return group;
    }

    public DialogueSystemNode CreateNode(DialogueSystemType dialogueType, Vector2 position)
    {
        Type nodeType = Type.GetType($"DialogueSystem{dialogueType}Node");
        DialogueSystemNode node = (DialogueSystemNode)Activator.CreateInstance(nodeType);

        node.Initialize(this, position);
        node.Draw();

        AddUngroupedNode(node);

        return node;
    }
    #endregion

    #region Callbacks
    private void OnElementsDeleted()
    {
        deleteSelection = (operationName, askUser) =>
        {
            Type groupType = typeof(DialogueSystemGroup);
            Type edgeType = typeof(Edge);

            List<DialogueSystemGroup> groupsToDelete = new List<DialogueSystemGroup>();
            List<Edge> edgesToDelete = new List<Edge>();
            List<DialogueSystemNode> nodesToDelete = new List<DialogueSystemNode>();

            foreach (GraphElement selectedelement in selection)
            {
                if (selectedelement is DialogueSystemNode node)
                {
                    nodesToDelete.Add(node);
                    continue;
                }

                if (selectedelement.GetType() == edgeType)
                {
                    Edge edge = (Edge)selectedelement;
                    edgesToDelete.Add(edge);
                    continue;
                }

                if (selectedelement.GetType() != groupType)
                {
                    continue;
                }

                DialogueSystemGroup group = (DialogueSystemGroup)selectedelement;
                
                groupsToDelete.Add(group);
            }

            foreach (DialogueSystemGroup group in groupsToDelete)
            {
                List<DialogueSystemNode> groupNodes = new List<DialogueSystemNode>();

                foreach (GraphElement groupElement in group.containedElements)
                {
                    if (!(groupElement is DialogueSystemNode))
                        continue;

                    DialogueSystemNode groupNode = (DialogueSystemNode)groupElement;
                    groupNodes.Add(groupNode);
                }

                group.RemoveElements(groupNodes);
                RemoveGroup(group);
                RemoveElement(group);
            }

            DeleteElements(edgesToDelete);

            foreach (DialogueSystemNode node in nodesToDelete)
            {
                if (node.Group != null)
                {
                    node.Group.RemoveElement(node);
                }

                RemoveUngroupedNode(node);
                node.DisconnectAllPorts();
                RemoveElement(node);
            }
        };
    }

    private void OnGroupElementsAdded()
    {
        elementsAddedToGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is DialogueSystemNode))
                {
                    continue;
                }

                DialogueSystemGroup nodeGroup = (DialogueSystemGroup)group;
                DialogueSystemNode node = (DialogueSystemNode)element;

                RemoveUngroupedNode(node);
                AddGroupedNode(node, nodeGroup);
            }
        };
    }

    private void OnGroupElementsRemoved()
    {
        elementsRemovedFromGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is DialogueSystemNode))
                    continue;

                DialogueSystemGroup dialogueSystemGroup = (DialogueSystemGroup)group;
                DialogueSystemNode node = (DialogueSystemNode)element;

                RemoveGroupedNode(node, group);
                AddUngroupedNode(node);
            }
        };
    }

    private void OnGroupRenamed()
    {
        groupTitleChanged = (group, newTitle) =>
        {
            DialogueSystemGroup dialogueSystemGroup = (DialogueSystemGroup)group;
            dialogueSystemGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
            RemoveGroup(dialogueSystemGroup);
            dialogueSystemGroup.oldTitle = dialogueSystemGroup.title;
            AddGroup(dialogueSystemGroup);
        };
    }

    private void OnGraphViewChange()
    {
        graphViewChanged = (changes) =>
        {
            if(changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    DialogueSystemNode nextNode = (DialogueSystemNode)edge.input.node;
                    DialogueSystemChoiceSaveData choiceData = (DialogueSystemChoiceSaveData)edge.output.userData;

                    choiceData.NodeID = nextNode.ID;
                }
            }
            
            if(changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach(GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                        continue;

                    Edge edge = (Edge)element;
                    DialogueSystemChoiceSaveData choiceData = (DialogueSystemChoiceSaveData)edge.output.userData;
                    choiceData.NodeID = "";
                }
            }

            return changes;
        };
    }
    #endregion

    #region Repeated Elements
    public void AddUngroupedNode(DialogueSystemNode node)
    {
        string nodeName = node.DialogueName.ToLower();

        if (!ungroupedNodes.ContainsKey(nodeName))
        {
            DialogueSystemNodeErrorData nodeErrorData = new DialogueSystemNodeErrorData();
            nodeErrorData.nodes.Add(node);
            ungroupedNodes.Add(nodeName, nodeErrorData);
            return;
        }

        List<DialogueSystemNode> ungroupedNodesList = ungroupedNodes[nodeName].nodes;

        ungroupedNodesList.Add(node);
        Color errorColor = ungroupedNodes[nodeName].errorData.color;
        node.SetErrorStyle(errorColor);

        if (ungroupedNodesList.Count == 2)
        {
            ++RepeatedNameAmount;
            ungroupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveUngroupedNode(DialogueSystemNode node)
    {
        string nodeName = node.DialogueName.ToLower();
        List<DialogueSystemNode> ungroupedNodesList = ungroupedNodes[nodeName].nodes;
        ungroupedNodesList.Remove(node);
        node.ResetStlye();

        if (ungroupedNodesList.Count == 1)
        {
            --RepeatedNameAmount;
            ungroupedNodesList[0].ResetStlye();
            return;
        }

        if (ungroupedNodesList.Count == 0)
        {
            ungroupedNodes.Remove(nodeName);
        }
    }

    private void AddGroup(DialogueSystemGroup group)
    {
        string groupName = group.title.ToLower();

        if (!groups.ContainsKey(groupName))
        {
            DialogueSystemGroupErrorData groupErrorData = new DialogueSystemGroupErrorData();
            groupErrorData.Groups.Add(group);
            groups.Add(groupName, groupErrorData);
            return;
        }

        List<DialogueSystemGroup> groupList = groups[groupName].Groups;

        groupList.Add(group);
        Color errorColor = groups[groupName].ErrorData.color;
        group.SetErrorStyle(errorColor);

        if (groupList.Count == 2)
        {
            ++RepeatedNameAmount;
            groupList[0].SetErrorStyle(errorColor);
        }
    }

    private void RemoveGroup(DialogueSystemGroup group)
    {
        string oldGroupName = group.oldTitle.ToLower();

        List<DialogueSystemGroup> groupList = groups[oldGroupName].Groups;

        groupList.Remove(group);
        group.ResetStyle();

        if (groupList.Count == 1)
        {
            --RepeatedNameAmount;
            groupList[0].ResetStyle();
            return;
        }

        if (groupList.Count == 0)
        {
            groups.Remove(oldGroupName);
        }
    }

    public void AddGroupedNode(DialogueSystemNode node, DialogueSystemGroup group)
    {
        string nodeName = node.DialogueName.ToLower();

        node.Group = group;

        if (!groupedNodes.ContainsKey(group))
        {
            groupedNodes.Add(group, new SerializableDictionary<string, DialogueSystemNodeErrorData>());
        }

        if (!groupedNodes[group].ContainsKey(nodeName))
        {
            DialogueSystemNodeErrorData nodeErrorData = new DialogueSystemNodeErrorData();
            nodeErrorData.nodes.Add(node);
            groupedNodes[group].Add(nodeName, nodeErrorData);
            return;
        }

        List<DialogueSystemNode> groupedNodesList = groupedNodes[group][nodeName].nodes;

        groupedNodes[group][nodeName].nodes.Add(node);
        Color errorColor = groupedNodes[group][nodeName].errorData.color;
        node.SetErrorStyle(errorColor);

        if (groupedNodes[group][nodeName].nodes.Count == 2)
        {
            ++RepeatedNameAmount;
            groupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveGroupedNode(DialogueSystemNode node, Group group)
    {
        string nodeName = node.DialogueName.ToLower();

        node.Group = null;

        List<DialogueSystemNode> groupedNodesList = groupedNodes[group][nodeName].nodes;
        groupedNodesList.Remove(node);
        node.ResetStlye();

        if (groupedNodesList.Count == 1)
        {
            --RepeatedNameAmount;
            groupedNodesList[0].ResetStlye();
            return;
        }

        if (groupedNodesList.Count == 0)
        {
            groupedNodes[group].Remove(nodeName);

            if (groupedNodes[group].Count == 0)
            {
                groupedNodes.Remove(group);
            }
        }
    }
    #endregion

    #region Elements Addition
    private void AddSearchWindow()
    {
        if (searchWindow == null)
        {
            searchWindow = ScriptableObject.CreateInstance<DialogueSystemSearchWindow>();
            searchWindow.Initialize(this);
        }

        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        this.AddStyleSheet(
            "DialogueSystem/DialogueSystemGraphViewStyles.uss",
            "DialogueSystem/DialogueSystemNodeStyles.uss"
            );
    }
    #endregion

    #region Utility
    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWondow = false)
    {
        Vector2 worldMousePosition = mousePosition;

        if (isSearchWondow)
        {
            worldMousePosition -= editorWindow.position.position;
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePosition;
    }
    #endregion
}
#endif