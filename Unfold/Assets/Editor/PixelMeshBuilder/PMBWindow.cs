using PMB;
using UnityEditor;
using UnityEngine;



public class PMBWindow : EditorWindow
{
    public Sprite[] Sprites;
    private Vector3 _scale;
    private MeshQuality _quality;
    private bool _inited = false;
    private bool _optimized = true;
    private bool _recalcNormals = true;

    [MenuItem("Window/PixelMeshBuilder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof (PMBWindow));
    }

    private void OnGUI()
    {
        if (!_inited)
        {
            Init();
            _inited = true;
        }
        GUILayout.BeginVertical();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("Sprites");
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties(); 
        _scale = EditorGUILayout.Vector3Field("Scale ", _scale);
        _quality = (MeshQuality)EditorGUILayout.EnumPopup("Mesh quality ",_quality);
        _optimized = EditorGUILayout.Toggle("Optimize mesh ", _optimized);
        _recalcNormals = EditorGUILayout.Toggle("Recalculate normals ", _recalcNormals);
        if(GUILayout.Button("Generate and Save")) GenerateAndSave();
        EditorGUILayout.HelpBox("Drag and drop sprites into the sprites field and click Generate and Save button.", MessageType.Info, true);
        EditorGUILayout.HelpBox("Sprite texture must be Read/Write Enabled. You can set this at image import settings window.", MessageType.Info);
        GUILayout.EndVertical();

    }

    private void Init()
    {
        _scale = new Vector3(1,1,1);
        _quality = MeshQuality.High;
        Sprites = new Sprite[0];
    }

    private void GenerateAndSave()
    {
        if (Sprites == null)
        {
            Debug.LogError("Sprites are not assigned!");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Save mesh asset", "Assets/", "mesh", "asset");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = path.Remove(path.Length - 6, 6);
        path = FileUtil.GetProjectRelativePath(path);
        var builder = new PixelMeshBuilder(_quality);
        foreach (var sprite in Sprites)
        {
            var mesh = builder.BuildMesh(sprite, _scale, _optimized, _recalcNormals);
            AssetDatabase.CreateAsset(mesh, path + "_" +sprite.name + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
    

