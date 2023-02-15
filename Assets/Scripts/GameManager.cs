using System.Collections.Generic;
using System.Linq;
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


	Stack<Node> path;
	// Update is called once per frame
	void Update()
	{
		if (!player.IsMoving)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitEnemy, 20, 1 << 7) &&
				hitEnemy.transform.TryGetComponent(out UnitEnemy enemy))
			{
				Node enemyNode = World.GetNodeFromWorldPosition(enemy.transform.position);
				int lowestCost = 10000;
				Stack<Node> lowestCostPath = null;
				for (int i = 0; i < 4; i++)
				{
					Node comparisonNode = World.GetAdjacentNode((World.Direction)i, enemyNode);
					if (comparisonNode == null) continue;

					Stack<Node> comparisonPath = Pathfinding.Pathfind(player.DestinationNode, enemyNode);

					if (GetPathCost(comparisonPath.ToArray()) < lowestCost)
					{
						lowestCostPath = comparisonPath;
					}
				}
				if (lowestCostPath != null)
				{
					player.SetPath(path);
					player.SetTargetedEnemy(enemy);
				}
			}
			else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitTile, 20, 1 << 6) &&
				hitTile.transform.TryGetComponent(out Tile tile))
			{
				if (Input.GetMouseButtonDown(0))
				{
					player.SetPath(path);
					cacheNode = null;
				}

				if (tile.node != cacheNode)
				{
					cacheNode = tile.node;
					path = Pathfinding.Pathfind(player.DestinationNode, hitTile.transform.GetComponent<Tile>().node);
					if (path == null) return;
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
