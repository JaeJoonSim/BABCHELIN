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
        DrawDialogueContainerArea();
        DrawFiltersArea();
        DrawDialogueGroupArea();
        DrawDialogueArea();
    }

    #region Draw Methods
    private void DrawDialogueContainerArea()
    {
        EditorGUILayout.LabelField("Dialogue Container", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dialogueContainerProperty);
        EditorGUILayout.Space(4);
    }

    private void DrawFiltersArea()
    {
        EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(groupedDialoguesProperty);
        EditorGUILayout.PropertyField(startingDialoguesOnlyProperty);
        EditorGUILayout.Space(4);
    }

    private void DrawDialogueGroupArea()
    {
        EditorGUILayout.LabelField("Dialogue Group", EditorStyles.boldLabel);
        selectedDialogueGroupIndexProperty.intValue = EditorGUILayout.Popup("Dialogue Group", selectedDialogueGroupIndexProperty.intValue, new string[] { });
        EditorGUILayout.PropertyField(dialogueGroupProperty);
        EditorGUILayout.Space(4);
    }

    private void DrawDialogueArea()
    {
        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        selectedDialogueIndexProperty.intValue = EditorGUILayout.Popup("Dialogue", selectedDialogueIndexProperty.intValue, new string[] { });
        EditorGUILayout.PropertyField(dialogueProperty);
        EditorGUILayout.Space(4);
    }
    #endregion
}
#endif