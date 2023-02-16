using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public UnitPlayer player;

	[field: SerializeField]
	public World World { get; private set; }

	[Header("Path Display")]
	[SerializeField]
	LineRenderer pathRenderer;
	[SerializeField]
	TextMeshPro pathCostText;
	Node cacheNode;
	UnitEnemy hoveredEnemy = null;

	Stack<Node> path = new();

	// Update is called once per frame
	void Update()
	{
		if (!player.IsMoving)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitTile, 20, 1 << 6) &&
				hitTile.transform.TryGetComponent(out TileBase tile))
			{
				if (Input.GetMouseButtonDown(0))
				{
					player.SetPath(path);
					player.SetTargetedEnemy(hoveredEnemy);
					cacheNode = null;
				}

				if (cacheNode == tile.Node) return;
				cacheNode = tile.Node;

				// Check if hit enemy
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitEnemy, 20, 1 << 7) &&
					hitEnemy.transform.TryGetComponent(out UnitEnemy enemy) &&
					World.GetNodeFromWorldPosition(enemy.transform.position) == tile.Node)
				{
					Node enemyNode = World.GetNodeFromWorldPosition(enemy.transform.position);

					// Find best path
					int lowestCost = 10000;
					Stack<Node> lowestCostPath = null;
					for (int i = 0; i < 4; i++)
					{
						// Cast to ITuple to iterate through
						if ((enemyNode.Neighbours as ITuple)[i] is not Node enemyNeighbourNode) continue;

						Stack<Node> comparisonPath = Pathfinding.Pathfind(player.DestinationNode, enemyNeighbourNode);
						if (comparisonPath == null) continue;

						int pathCost = GetPathCost(comparisonPath.ToArray());
						if (pathCost < lowestCost)
						{
							lowestCost = pathCost;
							lowestCostPath = comparisonPath;
						}
					}
					if (lowestCostPath == null) return;

					path = lowestCostPath;
					DisplayPath(path.ToArray());
					hoveredEnemy = enemy;
				}
				else
				{
					player.SetTargetedEnemy(null);
					Stack<Node> attemptedPath = Pathfinding.Pathfind(player.DestinationNode, tile.Node);
					if (attemptedPath == null) return;
					path = attemptedPath;
					DisplayPath(path.ToArray());
				}
			}
		}
	}


	void DisplayPath(Node[] path)
	{
		Vector3[] positions = path.Select(n => new Vector3(n.XPos + 0.5f, 0, n.YPos + 0.5f)).ToArray();

		pathRenderer.positionCount = positions.Length;
		pathRenderer.SetPositions(positions);

		int cost = GetPathCost(path);
		if (cost > 0)
		{
			pathCostText.text = cost.ToString();
			int middle = positions.Length / 2;
			pathCostText.transform.position = (positions.Length % 2 == 1 ? positions[middle] : (positions[middle] + positions[middle - 1]) / 2) + Vector3.up * 0.1f;
		}
		else
		{
			pathCostText.text = string.Empty;
		}

		pathRenderer.Simplify(0.1f);
	}

	int GetPathCost(Node[] path)
	{
		int cost = 0;
		for (int i = 1; i < path.Length; i++)
		{
			Node node = path[i];
			cost += node.MovementCost;
		}
		return cost;
	}
}
