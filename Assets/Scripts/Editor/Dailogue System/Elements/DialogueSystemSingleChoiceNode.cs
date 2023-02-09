using UnityEngine;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
public class DialogueSystemSingleChoiceNode : DialogueSystemNode
{
    public override void Initialize(DialogueSystemGraphView graphView, Vector2 position)
    {
        base.Initialize(graphView, position);

        DialogueType = DialogueSystemType.SingleChoice;

        DialogueSystemChoiceSaveData choiceData = new DialogueSystemChoiceSaveData()
        {
            Text = "Next Dialogue"
        };

        Choices.Add(choiceData);
    }

    public override void Draw()
    {
        base.Draw();

        foreach (DialogueSystemChoiceSaveData choice in Choices)
        {
            Port choicePort = this.CreatePort(choice.Text);

            choicePort.userData = choice;
            
            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }
}
#endif