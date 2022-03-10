using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarMovement))]
public class CarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CarMovement myScript = (CarMovement)target;
        
        if (GUILayout.Button("Your ButtonText"))
        {
            myScript.Print("hi");
        }
    }
}
