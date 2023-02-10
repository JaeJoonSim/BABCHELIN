using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#if UNITY_EDITOR
public class DialogueSystemSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueSystemGraphView graphView;
    private Texture2D indentationIcon;

    public void Initialize(DialogueSystemGraphView graphView)
    {
        this.graphView = graphView;
        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0, Color.clear);
        indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements")),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
            new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
            {
                level = 2,
                userData = DialogueSystemType.SingleChoice
            },
            new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
            {
                level = 2,
                userData = DialogueSystemType.MultipleChoice
            },
            new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
            new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
            {
                level = 2,
                userData = new Group()
            },
        };
        return searchTreeEntries;
    }


    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

        switch (SearchTreeEntry.userData)
        {
            case DialogueSystemType.SingleChoice:
                {
                    DialogueSystemSingleChoiceNode singleChoiceNode = (DialogueSystemSingleChoiceNode)graphView.CreateNode(DialogueSystemType.SingleChoice, localMousePosition);
                    graphView.AddElement(singleChoiceNode);
                    return true;
                }

            case DialogueSystemType.MultipleChoice:
                {
                    DialogueSystemMultipleChoiceNode multipleChoiceNode = (DialogueSystemMultipleChoiceNode)graphView.CreateNode(DialogueSystemType.MultipleChoice, localMousePosition);
                    graphView.AddElement(multipleChoiceNode);
                    return true;
                }

            case Group _:
                {
                    graphView.CreateGroup("Dialogue Group", localMousePosition);
                    return true;
                }

            default:
                return false;
        }
    }
}
#endif