using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemMultipleChoiceNode : DialogueSystemNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        DialogueType = DialogueSystemType.MultipleChoice;

        Choices.Add("New Choice");
    }

    public override void Draw()
    {
        base.Draw();

        Button addChoiceButton = DialogueSystemElementUtility.CreateButton("Add Choice", () =>
        {
            Port choicePort = CreateChoicePort("New Choice");

            Choices.Add("New Choice");

            outputContainer.Add(choicePort);
        });

        addChoiceButton.AddToClassList("ds-node__button");

        mainContainer.Insert(1, addChoiceButton);

        foreach (string choice in Choices)
        {
            Port choicePort = CreateChoicePort(choice);

            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }

    #region Elements Creation
    private Port CreateChoicePort(string choice)
    {
        Port choicePort = this.CreatePort();

        choicePort.portName = "";

        Button deleteChoiceButton = DialogueSystemElementUtility.CreateButton("X");

        deleteChoiceButton.AddToClassList("ds-node__button");

        TextField choiceTextField = DialogueSystemElementUtility.CreateTextField(choice);

        choiceTextField.AddClasses(
            "ds-node__textfield",
            "ds-node__choice-textfield",
            "ds-node__textfield__hidden"
            );

        choicePort.Add(choiceTextField);
        choicePort.Add(deleteChoiceButton);
        return choicePort;
    }
    #endregion
}
#endif