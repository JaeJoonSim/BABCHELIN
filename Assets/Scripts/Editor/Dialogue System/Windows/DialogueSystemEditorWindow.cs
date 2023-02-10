using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemEditorWindow : EditorWindow
{
    private DialogueSystemGraphView graphView;
    private string defaultFileName = "DialoguesFileName";
    private static TextField fileNameTextField;
    private Button saveButton;

    [MenuItem("Window/DialogueSystemEditorWindow")]
    public static void ShowExample()
    {
        GetWindow<DialogueSystemEditorWindow>("Dialogue Graph");
    }

    private void OnEnable()
    {
        AddGraphView();
        AddToolbar();
        AddStyles();
    }

    #region Elements Addtion
    private void AddGraphView()
    {
        graphView = new DialogueSystemGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void AddToolbar()
    {
        Toolbar toolbar = new Toolbar();
        fileNameTextField = DialogueSystemElementUtility.CreateTextField(defaultFileName, "File Name: ", callback =>
        {
            fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
        });
        
        saveButton = DialogueSystemElementUtility.CreateButton("Save", () => Save());

        Button clearButton = DialogueSystemElementUtility.CreateButton("Clear", () => Clear());
        Button resetButton = DialogueSystemElementUtility.CreateButton("Reset", () => ResetGraph());

        toolbar.Add(fileNameTextField);
        toolbar.Add(saveButton);
        toolbar.Add(clearButton);
        toolbar.Add(resetButton);

        toolbar.AddStyleSheet("DialogueSystem/DialogueSystemToolbarStyles.uss");

        rootVisualElement.Add(toolbar);
    }

    private void AddStyles()
    {
        rootVisualElement.AddStyleSheet("DialogueSystem/DialogueSystemVariables.uss");
    }
    #endregion

    #region Toolbar Actions
    private void Save()
    {
        if(string.IsNullOrEmpty(fileNameTextField.value))
        {
            EditorUtility.DisplayDialog(
                "Invalid file name",
                "please ensure the file name you've type in is valid",
                "Roger!"
                );

            return;
        }

        DialogueSystemIOUtility.Initialize(graphView, fileNameTextField.value);
        DialogueSystemIOUtility.Save();
    }

    private void Clear()
    {
        graphView.ClearGraph();
    }

    private void ResetGraph()
    {
        Clear();

        UpdateFileName(defaultFileName);
    }
    #endregion

    #region Utility Methods
    public static void UpdateFileName(string newFileName)
    {
        fileNameTextField.value = newFileName;
    }

    public void EnableSaving()
    {
        saveButton.SetEnabled(true);
    }

    public void DisableSaving()
    {
        saveButton.SetEnabled(false);
    }
    #endregion
}
#endif