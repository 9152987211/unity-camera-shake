using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenShake))]
public class ScreenShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty useCustomTarget = serializedObject.FindProperty("useCustomTarget");
        SerializedProperty customTarget = serializedObject.FindProperty("customTarget");

        DrawPropertiesExcluding(serializedObject, "useCustomTarget", "customTarget");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Target Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(useCustomTarget, new GUIContent("Use Custom Target"));

        if (useCustomTarget.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(customTarget);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
