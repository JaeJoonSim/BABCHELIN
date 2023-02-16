using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueSystemDialogue))]
public class DialogueSystemInspector : Editor
{
    private SerializedProperty dialogueContainerProperty;
    private SerializedProperty dialogueGroupProperty;
    private SerializedProperty dialogueProperty;

    private SerializedProperty groupedDialoguesProperty;
    private SerializedProperty startingDialoguesOnlyProperty;

    private SerializedProperty selectedDialogueGroupIndexProperty;
    private SerializedProperty selectedDialogueIndexProperty;

    private void OnEnable()
    {
        dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
        dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
        dialogueProperty = serializedObject.FindProperty("dialogue");

        groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
        startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");

        selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
        selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDialogueContainerArea();

        DialogueSystemContainerSO currentDialogueContainer = (DialogueSystemContainerSO)dialogueContainerProperty.objectReferenceValue;

        if (currentDialogueContainer == null)
        {
            StopDrawing("�ν����͸� Ȱ��ȭ �Ϸ��� Dialogue Container�� ������ �ּ���.");
            return;
        }

        DrawFiltersArea();

        if (groupedDialoguesProperty.boolValue)
        {
            List<string> dialogueGroupNames = currentDialogueContainer.GetDialogueGroupNames();

            if(dialogueGroupNames.Count == 0)
            {
                StopDrawing("�׷�ȭ�� ��ȭ�� �����ϴ�.");
                return;
            }
            DrawDialogueGroupArea(currentDialogueContainer, dialogueGroupNames);
        }

        DrawDialogueArea();

        serializedObject.ApplyModifiedProperties();
    }

    #region Draw Methods
    private void DrawDialogueContainerArea()
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue Container");
        dialogueContainerProperty.DrawPropertyField();
        DialogueSystemInspectorUtility.DrawSpace();
    }

    private void DrawFiltersArea()
    {
        DialogueSystemInspectorUtility.DrawHeader("Filters");
        groupedDialoguesProperty.DrawPropertyField();
        startingDialoguesOnlyProperty.DrawPropertyField();
        DialogueSystemInspectorUtility.DrawSpace();
    }

    private void DrawDialogueGroupArea(DialogueSystemContainerSO dialogueContainer, List<string> dialogueGroupNames)
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue Group");

        int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;
        DialogueSystemGroupSO oldDialogueGroup = (DialogueSystemGroupSO)dialogueGroupProperty.objectReferenceValue;

        bool isOldDialougeGroupNull = oldDialogueGroup == null;
        string oldDialougeGroupName = isOldDialougeGroupNull ? "" : oldDialogueGroup.GroupName;

        UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, oldSelectedDialogueGroupIndex, oldDialougeGroupName, isOldDialougeGroupNull);

        selectedDialogueGroupIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty.intValue, dialogueGroupNames.ToArray());

        string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
        DialogueSystemGroupSO selectedDialogueGroup = DialogueSystemIOUtility.LoadAsset<DialogueSystemGroupSO>($"Assets/Dialogue Data/Dialogues/{dialogueContainer.name}/Groups/{selectedDialogueGroupName}", selectedDialogueGroupName);
        dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;
        dialogueGroupProperty.DrawPropertyField();
        DialogueSystemInspectorUtility.DrawSpace();
    }

    private void DrawDialogueArea()
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue");
        selectedDialogueIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty.intValue, new string[] { });
        dialogueProperty.DrawPropertyField();
    }

    private void StopDrawing(string reason)
    {
        DialogueSystemInspectorUtility.DrawHelpBox(reason);
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Index Methods
    private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
    {
        if (isOldPropertyNull)
        {
            indexProperty.intValue = 0;
            return;
        }

        bool oldIndexIsOutBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
        bool oldNameIsDiffrentThanSelectedName = oldIndexIsOutBoundsOfNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

        if (oldNameIsDiffrentThanSelectedName)
        {
            if (optionNames.Contains(oldPropertyName))
            {
                indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
            }
            else
            {
                indexProperty.intValue = 0;
            }
        }
    }
    #endregion
}
#endif