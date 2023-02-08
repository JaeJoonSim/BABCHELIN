using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemGraphView : GraphView
{
    public DialogueSystemGraphView()
    {
        AddManipulators();
        AddGridBackground();

        AddStyles();
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // 순서 중요
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueSystemType.SingleChoice));
        this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueSystemType.MultipleChoice));
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueSystemType dialogueType)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, actionEvent.eventInfo.localMousePosition)))
            );

        return contextualMenuManipulator;
    }

    private DialogueSystemNode CreateNode(DialogueSystemType dialogueType, Vector2 position)
    {
        Type nodeType = Type.GetType($"DialogueSystem{dialogueType}Node");
        DialogueSystemNode node = (DialogueSystemNode)Activator.CreateInstance(nodeType);

        node.Initialize(position);
        node.Draw();

        return node;
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        StyleSheet graphViewstyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueSystemGraphViewStyles.uss");
        StyleSheet NodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueSystemNodeStyles.uss");

        styleSheets.Add(graphViewstyleSheet);
        styleSheets.Add(NodeStyleSheet);
    }
}
#endif