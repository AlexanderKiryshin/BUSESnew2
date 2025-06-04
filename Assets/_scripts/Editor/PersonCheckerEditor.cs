using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersonLoader))]
public class PersonCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PersonLoader personLoader = (PersonLoader)target;
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Checker Editor", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Check Person"))
        {
            personLoader.DeletePerson();
        }
    }
}
