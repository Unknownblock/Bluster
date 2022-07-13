using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GridSystem : MonoBehaviour
{
	[Header("Assignable")]
	public PathFinding pathFinding;
	public Mesh mesh;
	
	[Header("Important Variables")]
	public List<Node> path;
	public float currentLength;
	
	[Header("Color Settings")] 
	public Color pathColor;
	public Color unWalkableColor;
	public Color walkableColor;
	
	[Header("Settings")]
	public LayerMask unWalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public float wallDistance;
	public float distance;
	public float sizeY;

	private Node[,] grid;

	private float nodeDiameter;
	private int gridSizeX, gridSizeY;

	private void Awake()
	{
		CreateGrid();
		
		pathFinding = GetComponent<PathFinding>();
	}

	private void Update()
	{
		currentLength = CurrentLength();
		
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !Physics.CheckSphere(worldPoint, wallDistance, unWalkableMask);
				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	private float CurrentLength()
	{
		if (path.Count < 2)
			return currentLength = 0f;
        
		Vector3 previousCorner = path[0].worldPosition;
		
		float lengthSoFar = 0.0F;
		int i = 1;
		
		while (i < path.Count) {
			Vector3 currentCorner = path[i].worldPosition;
			lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
			previousCorner = currentCorner;
			i++;
		}

		return lengthSoFar;
	}

	public void CreateGrid()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !Physics.CheckSphere(worldPoint, wallDistance, unWalkableMask);
				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (grid != null)
		{
			foreach (Node everyNode in grid)
			{
				Gizmos.color = everyNode.isWalkable ? walkableColor : unWalkableColor;
				
				if (path != null)
				{
					if (path.Contains(everyNode))
					{
						Gizmos.color = pathColor;
					}
				}
				
				Gizmos.DrawMesh(mesh, everyNode.worldPosition, Quaternion.identity, new Vector3(1f, sizeY, 1f) * (nodeDiameter - distance));
			}
		}
	}
}