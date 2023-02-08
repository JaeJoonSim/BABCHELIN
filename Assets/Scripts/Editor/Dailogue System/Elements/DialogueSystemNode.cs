using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

#if UNITY_EDITOR
public class DialogueSystemNode : Node
{
    public string DialogueName { get; set; }
    public List<string> Choices { get; set; }
    public string Text { get; set; }
    public DialogueSystemType DialogueType { get; set; }

    public virtual void Initialize(Vector2 position)
    {
        DialogueName = "DialogueName";
        Choices = new List<string>();
        Text = "Dialogue Text";

        SetPosition(new Rect(position, Vector2.zero));

        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extention-container");
    }
    
    public virtual void Draw()
    {
        TextField dialogueNameTextField = DialogueSystemElementUtility.CreateTextField(DialogueName);

        dialogueNameTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__filename-textfield",
            "ds-node__textfield__hidden"
            );
        
        titleContainer.Insert(0, dialogueNameTextField);

        Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

        inputContainer.Add(inputPort);

        VisualElement customDataContainer = new VisualElement();

        customDataContainer.AddToClassList("ds-node__custom-data-container");

        Foldout textFoldout = DialogueSystemElementUtility.CreateFoldout("Dialogue Text");

        TextField textTextField = DialogueSystemElementUtility.CreateTextArea(Text);

        textTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__quote-textfield"
            );

        textFoldout.Add(textTextField);
        customDataContainer.Add(textFoldout);
        extensionContainer.Add(customDataContainer);
    }
}
#endif