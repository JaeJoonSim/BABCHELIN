using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationSync))]
public class LocalizationSyncEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var component = (LocalizationSync)target;

        if (GUILayout.Button("Sync"))
        {
            component.Sync();
        }
    }
}