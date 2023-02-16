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
        
        if(dialogueContainerProperty.objectReferenceValue == null)
        {
            StopDrawing("인스펙터를 활성화 하려면 Dialogue Container를 선택해 주세요.");
            return;
        }

        DrawFiltersArea();
        DrawDialogueGroupArea();
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

    private void DrawDialogueGroupArea()
    {
        DialogueSystemInspectorUtility.DrawHeader("Dialogue Group");
        selectedDialogueGroupIndexProperty.intValue = DialogueSystemInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty.intValue, new string[] { });
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
}
#endif