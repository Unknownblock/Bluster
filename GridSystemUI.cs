using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var gridSystem = target as GridSystem;
        if (gridSystem != null)
        {
            if (GUILayout.Button("Create And Initialize Grid"))
            {

                gridSystem.CreateGrid();
            }
        }
    }
}
