using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Pathfinding
{
	static public Stack<Node> Pathfind(Node start, Node destination)
	{
		bool foundPath = false;

		/// The set of nodes to visit the algorithm is aware of 
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
				// Get the neighbour and it is not null (off map), is walkable, and has not already beem visited
				// Cast to ITuple to iterate through
				if ((current.Neighbours as ITuple)[i] is not Node neighbour || !neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

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

	static int GetDistance(Node a, Node b) => Mathf.Abs(a.XPos - b.XPos) + Mathf.Abs(a.YPos - b.YPos);
}

[System.Serializable]
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
	public (Node left, Node right, Node up, Node down) Neighbours { get; private set; }

	public void Init(int xPos, int yPos, int movementCost, bool walkable)
	{
		XPos = xPos;
		YPos = yPos;
		MovementCost = movementCost;
		IsWalkable = walkable;
	}

	public void SetNeighbours(Node neighbourLeft, Node neighbourRight, Node neighbourUp, Node neighbourDown) => Neighbours = (neighbourLeft, neighbourRight, neighbourUp, neighbourDown);

	public void SetParent(Node node) => Parent = node;

	public void SetWalkable(bool walkable) => IsWalkable = walkable;

	public void SetValues(int gCost, int hCost, Node parent)
	{
		GCost = gCost;
		HCost = hCost;
		SetParent(parent);
	}
}