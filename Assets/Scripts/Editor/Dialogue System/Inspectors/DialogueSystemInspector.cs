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
            StopDrawing("인스펙터를 활성화 하려면 Dialogue Container를 선택해 주세요.");
            return;
        }

        DrawFiltersArea();

        List<string> dialogueNames;
        string dialogueFolderPath = $"Assets/Dialogue Data/Dialogues/{currentDialogueContainer.name}";
        string dialogueInfoMessage;

        if (groupedDialoguesProperty.boolValue)
        {
            List<string> dialogueGroupNames = currentDialogueContainer.GetDialogueGroupNames();

            if(dialogueGroupNames.Count == 0)
            {
                StopDrawing("그룹화된 대화가 없습니다.");
                return;
            }
            DrawDialogueGroupArea(currentDialogueContainer, dialogueGroupNames);

            DialogueSystemGroupSO dialogueGroup = (DialogueSystemGroupSO)dialogueGroupProperty.objectReferenceValue;
            dialogueNames = currentDialogueContainer.GetGroupedDialogueNames(dialogueGroup);
            dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";
            dialogueInfoMessage = "이 Dialogue Group에 대화가 없습니다.";
        }
        else
        {
            dialogueNames = currentDialogueContainer.GetUnGroupedDialogueNames();
            dialogueFolderPath += "/Global/Dialogues";
            dialogueInfoMessage = "이 Dialogue에는 대화가 없습니다.";
        }

        if(dialogueNames.Count == 0)
        {
            StopDrawing(dialogueInfoMessage);
            return;
        }

        DrawDialogueArea(dialogueNames, dialogueFolderPath);

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

    private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue");

        int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;
        DialogueSystemDialogueSO oldDialogue = (DialogueSystemDialogueSO)dialogueProperty.objectReferenceValue;
        bool isOldDialougeNull = oldDialogue == null;
        string oldDialougeName = isOldDialougeNull ? "" : oldDialogue.DialogueName;

        UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialougeName, isOldDialougeNull);

        selectedDialogueIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty.intValue, dialogueNames.ToArray());

        string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
        DialogueSystemDialogueSO selectedDialogue = DialogueSystemIOUtility.LoadAsset<DialogueSystemDialogueSO>(dialogueFolderPath, selectedDialogueName);
        dialogueProperty.objectReferenceValue = selectedDialogue;

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