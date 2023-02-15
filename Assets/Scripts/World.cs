using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
	// NOTE: Nodes are traversal objects, tiles are GameObjects
	public enum Direction { Left, Right, Up, Down }
	const int WorldXSize = 11;
	const int WorldYSize = 11;

	// The prefabs of the tiles that can be spawned
	public Tile[] spawnableTilePrefabs;

	// The array of tiles in the world
	Node[,] worldNodes;

	// Start is called before the first frame update
	void Start()
	{
		Generate();
	}

	void Generate()
	{
		worldNodes = new Node[WorldXSize, WorldYSize];
		// Spawn tiles
		GameObject world = new("World");
		for (int y = 0; y < WorldYSize; y++)
		{
			GameObject row = new($"Row {y}");
			row.transform.parent = world.transform;
			for (int x = 0; x < WorldXSize; x++)
			{
				Tile t = Instantiate(spawnableTilePrefabs[Random.Range(0, spawnableTilePrefabs.Length)], new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(90, 0, 0));
				worldNodes[x, y] = t.node;
				t.node.Init(x, y, t.MovementCost, t.IsWalkable);
				t.name = $"{x}, {y}";
				t.transform.parent = row.transform;
			}
		}
	}

	public Stack<Node> Pathfind(Node start, Node destination)
	{
		bool foundPath = false;

		// The set of nodes to visit the algorithm is aware of 
		Queue<Node> openSet = new();
		// The set of visited nodes
		List<Node> closedSet = new();
		start.SetParent(start);

		// Return if destination is not visitable (TODO: nav to closest tile?)
		if (!destination.IsWalkable)
		{
			//Debug.LogWarning("End tile unwalkable");
			return null;
		}

		// Prepare the starting node
		openSet.Enqueue(start);

		// Until we run out of nodes
		while (openSet.Count > 0)
		{
			Node current = openSet.Dequeue();
			closedSet.Add(current);

			// Destination found!
			if (current == destination)
			{
				foundPath = true;
				break;
			}

			// iterate over the neighbours
			for (int i = 0; i < 4; i++)
			{
				// Get the neighbour
				Node neighbour = AdjacentNode((Direction)i, current, worldNodes);
				// Check the neighbor is not null (off map), is walkable, and has not already beem visited
				if (neighbour == null || !neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

				// Calculate the new gCost
				int newGCost = current.GCost + GetDistance(current, neighbour) + neighbour.MovementCost;

				// Check if the neightbour is not in the set to visit or if new calculation is less than the existing 
				if (newGCost < neighbour.GCost || !openSet.Contains(neighbour))
				{
					neighbour.SetValues(newGCost, GetDistance(destination, neighbour), current);

					// Remove the old version of the neighbour if it exists
					if (openSet.Contains(neighbour))
					{
						openSet = new(openSet.Where(node => node != neighbour));
					}
					// Add the new/updated neighbour
					openSet.Enqueue(neighbour);
				}
			}
			// Sort the openSet by fCost
			openSet = new(openSet.OrderBy(node => node.FCost));
		}

		// If path found, 
		if (foundPath)
		{
			Stack<Node> path = new();
			Node pathingNode = destination;
			while (pathingNode != start)
			{
				path.Push(pathingNode);
				pathingNode = pathingNode.Parent;
			}
			path.Push(start);
			return path;
		}
		else
		{
			// At this point, run a greedy best first search and re-run Pathfind() to the lowest?
			//Debug.LogWarning("No Path Found");
			return null;
		}
	}

	int GetDistance(Node a, Node b) => Mathf.Abs(a.XPos - b.XPos) + Mathf.Abs(a.YPos - b.YPos);

	public Node GetNodeFromWorldPosition(Vector3 pos) => (pos.x >= WorldXSize || pos.z >= WorldYSize) ? null : worldNodes[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)];

	public Node AdjacentNode(Direction direction, Node sourceNode, Node[,] allNodes)
	{
		return direction switch
		{
			Direction.Left => sourceNode.XPos - 1 < 0 ? null : allNodes[sourceNode.XPos - 1, sourceNode.YPos],
			Direction.Right => sourceNode.XPos + 1 >= WorldXSize ? null : allNodes[sourceNode.XPos + 1, sourceNode.YPos],
			Direction.Up => sourceNode.YPos + 1 >= WorldYSize ? null : allNodes[sourceNode.XPos, sourceNode.YPos + 1],
			Direction.Down => sourceNode.YPos - 1 < 0 ? null : allNodes[sourceNode.XPos, sourceNode.YPos - 1],
			_ => null,
		};
	}
}
[Serializable]
public class Node
{
	public bool IsWalkable { get; set; }
	public int XPos { get; private set; }
	public int YPos { get; private set; }

	public int MovementCost { get; private set; } = 1;

	/// <summary>
	/// Distance to start node
	/// </summary>
	public int GCost { get; private set; }
	/// <summary>
	/// Distance from end node
	/// </summary>
	public int HCost { get; private set; }
	/// <summary>
	/// Total cost
	/// </summary>
	public int FCost => GCost + HCost;

	public Node Parent { get; private set; }

	public void Init(int xPos, int yPos, int movementCost, bool walkable)
	{
		XPos = xPos;
		YPos = yPos;
		MovementCost = movementCost;
		IsWalkable = walkable;
	}

	public void SetParent(Node node) => Parent = node;

	public void SetValues(int gCost, int hCost, Node parent)
	{
		GCost = gCost;
		HCost = hCost;
		SetParent(parent);
	}
}