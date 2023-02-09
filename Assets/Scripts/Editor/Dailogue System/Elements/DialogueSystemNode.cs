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
    public DialogueSystemGroup Group { get; set; }

    private DialogueSystemGraphView graphView;
    private Color defaultBackgroundColor;

    public virtual void Initialize(DialogueSystemGraphView graphView, Vector2 position)
    {
        DialogueName = "DialogueName";
        Choices = new List<string>();
        Text = "Dialogue Text";
        this.graphView = graphView;
        
        defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

        SetPosition(new Rect(position, Vector2.zero));

        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extention-container");
    }

    public virtual void Draw()
    {
        TextField dialogueNameTextField = DialogueSystemElementUtility.CreateTextField(DialogueName, null, callback =>
        {
            TextField target = (TextField)callback.target;
            target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

            if(Group == null)
            {
                graphView.RemoveUngroupedNode(this);
                DialogueName = target.value;
                graphView.AddUngroupedNode(this);
                return;
            }

            DialogueSystemGroup currentGroup = Group;
            graphView.RemoveGroupedNode(this, Group);
            DialogueName = callback.newValue;
            graphView.AddGroupedNode(this, currentGroup);
        });

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

    #region Override Methods
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
        evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());
        
        base.BuildContextualMenu(evt);
    }
    #endregion

    #region Utility Methods
    public void DisconnectAllPorts()
    {
        DisconnectInputPorts();
        DisconnectOutputPorts();
    }
    
    private void DisconnectInputPorts()
    {
        DisconnectPorts(inputContainer);
    }
    
    private void DisconnectOutputPorts()
    {
        DisconnectPorts(outputContainer);
    }

    private void DisconnectPorts(VisualElement container)
    {
        foreach (Port port in container.Children())
        {
            if (!port.connected)
                continue;

            graphView.DeleteElements(port.connections);
        }
    }

    public void SetErrorStyle(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }

    public void ResetStlye()
    {
        mainContainer.style.backgroundColor = defaultBackgroundColor;
    }
    #endregion
}
#endif