using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vehicle))]
public class CarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Vehicle myScript = (Vehicle)target;

        if (GUILayout.Button("Create Wheel Details"))
        {
            myScript.GenerateWheels();
        }
    }
}
