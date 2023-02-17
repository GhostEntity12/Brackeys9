using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	//Node cacheNode;
	UnitEnemy hoveredEnemy = null;

	Node[] path;

	bool gameOver;
	[field: SerializeField]
	public DoomsdayClock Clock { get; private set; }

	private void Start()
	{
		World.Generate();
	}

	// Update is called once per frame
	void Update()
	{
		if (!gameOver && !player.IsMoving)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitTile, 20, 1 << 6) &&
				hitTile.transform.TryGetComponent(out TileBase tile))
			{
				if (Input.GetMouseButtonDown(0))
				{
					player.SetPath(new(path.Reverse()));
					player.SetTargetedEnemy(hoveredEnemy);
					//cacheNode = null;
				}

				//if (cacheNode == tile.Node) return;
				//cacheNode = tile.Node;

				// Check if hit enemy
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitEnemy, 20, 1 << 7) &&
					hitEnemy.transform.TryGetComponent(out UnitEnemy enemy) &&
					!enemy.IsDead &&
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

					path = lowestCostPath.ToArray();
					DisplayPath(path);
					hoveredEnemy = enemy;
				}
				else
				{
					player.SetTargetedEnemy(null);
					Stack<Node> attemptedPath = Pathfinding.Pathfind(player.DestinationNode, tile.Node);
					if (attemptedPath == null) return;
					path = attemptedPath.ToArray();
					DisplayPath(path);
				}
			}
		}
	}


	void DisplayPath(Node[] path)
	{
		Vector3[] positions = path.Select(n => new Vector3(n.XPos + 0.5f, 0, n.YPos + 0.5f)).ToArray();

		pathRenderer.positionCount = positions.Length;
		pathRenderer.SetPositions(positions);

		pathRenderer.Simplify(0.1f);
		Clock.ShowPreview(GetPathCost(path));
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

	public void GenerateNewWorld()
	{
		Debug.Log("Make a new one");
		World.Destroy();
		World.Generate();
	}

	public void GameOver()
	{
		Debug.Log("Game Over!");
		gameOver = true;
	}
}
