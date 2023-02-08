using System;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemEditorWindow : EditorWindow
{
    [MenuItem("Window/DialogueSystemEditorWindow")]
    public static void ShowExample()
    {
        GetWindow<DialogueSystemEditorWindow>("Dialogue Graph");
    }

    private void OnEnable()
    {
        AddGraphView();

        AddStyles();
    }

    private void AddGraphView()
    {
        DialogueSystemGraphView graphView = new DialogueSystemGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void AddStyles()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueSystemVariables.uss");

        rootVisualElement.styleSheets.Add(styleSheet);
    }
}
#endif