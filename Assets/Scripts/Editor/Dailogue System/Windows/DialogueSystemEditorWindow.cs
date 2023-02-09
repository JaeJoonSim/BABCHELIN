using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemEditorWindow : EditorWindow
{
    private string defaultFileName = "DialoguesFileName";
    private TextField fileNameTextField;
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
        DialogueSystemGraphView graphView = new DialogueSystemGraphView(this);
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
        saveButton = DialogueSystemElementUtility.CreateButton("Save");

        toolbar.Add(fileNameTextField);
        toolbar.Add(saveButton);

        toolbar.AddStyleSheet("DialogueSystem/DialogueSystemToolbarStyles.uss");

        rootVisualElement.Add(toolbar);
    }

    private void AddStyles()
    {
        rootVisualElement.AddStyleSheet("DialogueSystem/DialogueSystemVariables.uss");
    }
    #endregion

    #region Utility Methods
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