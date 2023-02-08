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

    #region Elements Addtion
    private void AddGraphView()
    {
        DialogueSystemGraphView graphView = new DialogueSystemGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void AddStyles()
    {
        rootVisualElement.AddStyleSheet("DialogueSystem/DialogueSystemVariables.uss");
    }
    #endregion
}
#endif