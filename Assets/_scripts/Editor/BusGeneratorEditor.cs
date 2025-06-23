using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BusGenerator))]
public class BusGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BusGenerator generator = (BusGenerator)target;
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bus Generation Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Generate Small Buses"))
        {
            generator.FillAreas(generator.smallBusPrefab, generator.smallBusCount, generator.SmallBusSize);
        }

        if (GUILayout.Button("Generate Medium Buses"))
        {
            generator.FillAreas(generator.mediumBusPrefab, generator.mediumBusCount, generator.MediumBusSize);
        }

        if (GUILayout.Button("Generate Large Buses"))
        {
            generator.FillAreas(generator.largeBusPrefab, generator.largeBusCount, generator.LargeBusSize);
        }if (GUILayout.Button("Clear Buses"))
        {
            generator.ClearBusesList();
        }
        if (GUILayout.Button("LoadBusData"))
        {
            generator.LoadBusData();
        }
        if (GUILayout.Button("SaveBusData"))
        {
            generator.SaveBusData();
        }

    }
}