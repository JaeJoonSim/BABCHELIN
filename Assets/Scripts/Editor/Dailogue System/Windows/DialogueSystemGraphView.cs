using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
public class DialogueSystemGraphView : GraphView
{
    private DialogueSystemEditorWindow editorWindow;
    private DialogueSystemSearchWindow searchWindow;

    private SerializableDictionary<string, DialogueSystemNodeErrorData> ungroupedNodes;

    public DialogueSystemGraphView(DialogueSystemEditorWindow editorWindow)
    {
        this.editorWindow = editorWindow;

        ungroupedNodes = new SerializableDictionary<string, DialogueSystemNodeErrorData>();

        // 순서 중요
        AddManipulators();
        AddSearchWindow();
        AddGridBackground();

        OnElementsDeleted();

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

        this.AddManipulator(CreateGroupContextualMenu());
    }

    private IManipulator CreateGroupContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("Dialogue Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

        return contextualMenuManipulator;
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueSystemType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

        return contextualMenuManipulator;
    }
    #endregion

    #region Elements Creation
    public Group CreateGroup(string title, Vector2 localMousePosition)
    {
        Group group = new Group()
        {
            title = title
        };
        group.SetPosition(new Rect(localMousePosition, Vector2.zero));

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
            List<DialogueSystemNode> nodesToDelete = new List<DialogueSystemNode>();

            foreach (GraphElement element in selection)
            {
                if(element is DialogueSystemNode)
                {
                    nodesToDelete.Add((DialogueSystemNode)element);
                    continue;
                }
            }

            foreach (DialogueSystemNode node in nodesToDelete)
            {
                RemoveUngroupedNode(node);
                RemoveElement(node);
            }
        };
    }
    #endregion

    #region Repeated Elements
    public void AddUngroupedNode(DialogueSystemNode node)
    {
        string nodeName = node.DialogueName;

        if(!ungroupedNodes.ContainsKey(nodeName))
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
            ungroupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveUngroupedNode(DialogueSystemNode node)
    {
        string nodeName = node.DialogueName;
        List<DialogueSystemNode> ungroupedNodesList = ungroupedNodes[nodeName].nodes;
        ungroupedNodesList.Remove(node);
        node.ResetStlye();

        if(ungroupedNodesList.Count == 1)
        {
            ungroupedNodesList[0].ResetStlye();
            return;
        }

        if(ungroupedNodesList.Count == 0)
        {
            ungroupedNodes.Remove(nodeName);
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