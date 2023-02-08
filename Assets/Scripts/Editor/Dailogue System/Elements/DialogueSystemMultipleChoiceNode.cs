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

        Button addChoiceButton = new Button()
        { 
            text = "Add Choice"
        };

        addChoiceButton.AddToClassList("ds-node__button");

        mainContainer.Insert(1, addChoiceButton);

        foreach (string choice in Choices)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.portName = "";

            Button deleteChoiceButton = new Button()
            {
                text = "X"
            };

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = new TextField()
            {
                value = choice
            };

            choiceTextField.AddToClassList("ds-node__textfield");
            choiceTextField.AddToClassList("ds-node__choice-textfield");
            choiceTextField.AddToClassList("ds-node__textfield__hidden");

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }
}
#endif