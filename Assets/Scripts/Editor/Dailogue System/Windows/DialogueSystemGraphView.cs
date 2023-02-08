using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DialogueSystemGraphView : GraphView
{
    public DialogueSystemGraphView()
    {
        AddManipulators();
        AddGridBackground();

        CreateNode();

        AddStyles();
    }

    private void CreateNode()
    {
        DialogueSystemNode node = new DialogueSystemNode();

        node.Initialize();
        node.Draw();

        AddElement(node);
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueSystemGraphViewStyles.uss");

        styleSheets.Add(styleSheet);
    }
}
#endif