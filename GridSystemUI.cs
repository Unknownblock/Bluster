using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridSystemUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var gridSystem = target as GridSystem;
        
        if (GUILayout.Button("Create And Initialize Grid"))
        {
            if (gridSystem != null)
            {
                gridSystem.CreateGrid();
            }
        }
    }
}
