using PMB;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PMBComponent))]
[CanEditMultipleObjects]

public class PMBComponentEditor : Editor
{

    SerializedProperty Sprite;
    SerializedProperty Scale;
    SerializedProperty Quality;
    SerializedProperty Optimized;
    SerializedProperty RecalcNormals;
    SerializedProperty RunOnStart;
    SerializedProperty Material;
    SerializedProperty UseAutoMaterial;
    SerializedProperty Shader;
    SerializedProperty UseUnlit;
    
    void OnEnable()
    {
        Sprite = serializedObject.FindProperty("Sprite");
        Scale = serializedObject.FindProperty("Scale");
        Quality = serializedObject.FindProperty("Quality");
        Optimized = serializedObject.FindProperty("Optimized");
        RecalcNormals = serializedObject.FindProperty("RecalcNormals");
        RunOnStart = serializedObject.FindProperty("RunOnStart");
        Material = serializedObject.FindProperty("Material");
        UseAutoMaterial = serializedObject.FindProperty("UseAutoMaterial");
        Shader = serializedObject.FindProperty("Shader");
        UseUnlit = serializedObject.FindProperty("UseUnlit");
    }

    public override void OnInspectorGUI()
    {
        if(Application.isPlaying) return;
        serializedObject.Update();

        EditorGUILayout.PropertyField(Sprite);
        EditorGUILayout.PropertyField(Scale);
        EditorGUILayout.PropertyField(Quality);
        EditorGUILayout.PropertyField(Optimized);
        EditorGUILayout.PropertyField(RecalcNormals);
        EditorGUILayout.PropertyField(RunOnStart);
        EditorGUILayout.PropertyField(UseAutoMaterial);
        if (!UseAutoMaterial.boolValue)
        {
            EditorGUILayout.PropertyField(Material);
        }
        else
        {
            EditorGUILayout.PropertyField(UseUnlit);
            if (!UseUnlit.boolValue)
            {
                EditorGUILayout.PropertyField(Shader);
            }
        }
        EditorGUILayout.HelpBox("Sprite texture must be Read/Write Enabled. You can set this at image import settings window.",MessageType.Info);
        serializedObject.ApplyModifiedProperties();
    }
}

