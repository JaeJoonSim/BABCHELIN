using UnityEngine;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
public class DialogueSystemSingleChoiceNode : DialogueSystemNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        DialogueType = DialogueSystemType.SingleChoice;

        Choices.Add("Next Dialogue");
    }

    public override void Draw()
    {
        base.Draw();

        foreach (string choice in Choices)
        {
            Port choicePort = this.CreatePort(choice);

            choicePort.portName = choice;
            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }
}
#endif