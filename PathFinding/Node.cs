using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public readonly int gridX;
    public readonly int gridY;

    public readonly bool isWalkable;

    public int gCost;
    public int hCost;
    public Node parent;
    
    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        isWalkable = walkable;
        worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}