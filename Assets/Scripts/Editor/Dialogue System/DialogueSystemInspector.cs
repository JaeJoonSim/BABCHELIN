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
        DialogueSystemContainerSO dialogueContainer = (DialogueSystemContainerSO)dialogueContainerProperty.objectReferenceValue;

        if (dialogueContainer == null)
        {
            StopDrawing("Select a Dialogue Container to see the rest of the Inspector");
            return;
        }

        DrawFiltersArea();

        List<string> dialogueNames;
        string dialogueFolderPath = $"Assets/Dialogue Data/Dialogues/{dialogueContainer.FileName}";
        string dialogueInfoMessege;

        if (groupedDialoguesProperty.boolValue)
        {
            List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();
            
            if (dialogueGroupNames.Count == 0)
            {
                StopDrawing("There are no Dialogue Groups in the Dialogue Container");
                return;
            }
            
            DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);

            DialogueSystemGroupSO dialogueGroup = (DialogueSystemGroupSO)dialogueGroupProperty.objectReferenceValue;
            dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup);
            dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";
            dialogueInfoMessege = "There are no Dialogues in this Dialogue Group.";
        }
        else
        {
            dialogueNames = dialogueContainer.GetUngroupedDialogueNames();
            dialogueFolderPath += "/Global/Dialogues";
            dialogueInfoMessege = "There are no Ungrouped Dialogues in this Dialogue Group.";
        }

        if (dialogueNames.Count == 0)
        {
            StopDrawing(dialogueInfoMessege);
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

        bool isOldDialogueGroupNull = oldDialogueGroup == null;
        string OldDialogueGroupName = isOldDialogueGroupNull ? "" : oldDialogueGroup.GroupName;

        UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueIndexProperty, oldSelectedDialogueGroupIndex, OldDialogueGroupName, isOldDialogueGroupNull);

        selectedDialogueGroupIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty, dialogueGroupNames.ToArray());
        string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
        DialogueSystemGroupSO selectedDailogueGroup = DialogueSystemIOUtility.LoadAsset<DialogueSystemGroupSO>($"Assets/Dialogue Data/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}", selectedDialogueGroupName);
        dialogueGroupProperty.objectReferenceValue = selectedDailogueGroup;
        dialogueGroupProperty.DrawPropertyField();
        DialogueSystemInspectorUtility.DrawSpace();
    }

    private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue");
        int oldSelectdDialogueIndex = selectedDialogueIndexProperty.intValue;
        DialogueSystemDialogueSO oldDialogue = (DialogueSystemDialogueSO)dialogueProperty.objectReferenceValue;

        bool isOldDialogueNull = oldDialogue == null;
        string OldDialogueName = isOldDialogueNull ? "" : oldDialogue.DialogueName;

        UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectdDialogueIndex, OldDialogueName, isOldDialogueNull);

        selectedDialogueIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty, dialogueNames.ToArray());
        string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
        DialogueSystemDialogueSO selectedDialogue = DialogueSystemIOUtility.LoadAsset<DialogueSystemDialogueSO>(dialogueFolderPath, selectedDialogueName);
        dialogueProperty.objectReferenceValue = selectedDialogue;

        dialogueProperty.DrawPropertyField();
    }

    private void StopDrawing(string reason)
    {
        DialogueSystemInspectorUtility.DrawHelpBox(reason, MessageType.Info);
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

        bool oldIndexIsOutOfBoundsNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
        bool oldNameIsDiffrentThanSelectedName = oldIndexIsOutOfBoundsNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

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