using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bus))]
public class BusEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Bus generator = (Bus)target;
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bus Rotation", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Rotate180"))
        {
            generator.Rotate();
        }
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Rotate90"))
        {
            generator.Rotate90();
        }
    }
}
