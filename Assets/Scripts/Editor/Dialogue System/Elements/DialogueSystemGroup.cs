using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

#if UNITY_EDITOR
public class DialogueSystemGroup : Group
{
    public string ID { get; set; }
    public string oldTitle { get; set; }

    private Color defaultBorderColor;
    private float defaultBorderWidth;

    public DialogueSystemGroup(string groupTitle, Vector2 position)
    {
        ID = Guid.NewGuid().ToString();
        title = groupTitle;
        oldTitle = groupTitle;

        SetPosition(new Rect(position, Vector2.zero));
        defaultBorderColor = contentContainer.style.borderBottomColor.value;
        defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
    }

    public void SetErrorStyle(Color color)
    {
        contentContainer.style.borderBottomColor = color;
        contentContainer.style.borderBottomWidth = 2f;
    }
    
    public void ResetStyle()
    {
        contentContainer.style.borderBottomColor = defaultBorderColor;
        contentContainer.style.borderBottomWidth = defaultBorderWidth;
    }
}
#endif