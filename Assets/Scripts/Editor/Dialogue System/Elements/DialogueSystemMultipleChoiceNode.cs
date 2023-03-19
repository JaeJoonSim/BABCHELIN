using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemMultipleChoiceNode : DialogueSystemNode
{
    public override void Initialize(string nodeName, DialogueSystemGraphView graphView , Vector2 position)
    {
        base.Initialize(nodeName, graphView, position);

        DialogueType = DialogueSystemType.MultipleChoice;

        DialogueSystemChoiceSaveData choiceData = new DialogueSystemChoiceSaveData()
        {
            Text = "Next Choice"
        };

        Choices.Add(choiceData);
    }

    public override void Draw()
    {
        base.Draw();

        Button addChoiceButton = DialogueSystemElementUtility.CreateButton("Add Choice", () =>
        {
            DialogueSystemChoiceSaveData choiceData = new DialogueSystemChoiceSaveData()
            {
                Text = "Next Choice"
            };

            Choices.Add(choiceData);
            Port choicePort = CreateChoicePort(choiceData);

            outputContainer.Add(choicePort);
        });

        addChoiceButton.AddToClassList("ds-node__button");

        mainContainer.Insert(1, addChoiceButton);

        foreach (DialogueSystemChoiceSaveData choice in Choices)
        {
            Port choicePort = CreateChoicePort(choice);

            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }

    #region Elements Creation
    private Port CreateChoicePort(object userData)
    {
        Port choicePort = this.CreatePort();
        choicePort.userData = userData;
        DialogueSystemChoiceSaveData choiceData = (DialogueSystemChoiceSaveData)userData;

        choicePort.portName = "";

        Button deleteChoiceButton = DialogueSystemElementUtility.CreateButton("X", () =>
        {
            if(Choices.Count == 1)
            {
                return;
            }

            if(choicePort.connected)
            {
                graphView.DeleteElements(choicePort.connections);
            }

            Choices.Remove(choiceData);
            graphView.RemoveElement(choicePort);
        });

        deleteChoiceButton.AddToClassList("ds-node__button");

        TextField choiceTextField = DialogueSystemElementUtility.CreateTextField(choiceData.Text, null, callback =>
        {
            choiceData.Text = callback.newValue;
        });

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