using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField]
	World worldPrefab;
	[SerializeField]
	UnitPlayer playerPrefab;
	[SerializeField]
	DoomsdayClock clockPrefab;
	[SerializeField]
	CrayonPath crayonPathPrefab;

	[SerializeField]
	HealthDisplay healthDisplayPrefab;
	[SerializeField]
	StatsDisplay statsDisplayPrefab;
	[SerializeField]
	XPDisplay xpDisplayPrefab;


	public World World { get; private set; }
	UnitPlayer player;
	DoomsdayClock clock;
	CrayonPath crayonPath;


	UnitEnemy hoveredEnemy = null;
	Stack<Node> path;
	bool gameEnded;
	public bool InEndlessMode { get; private set; }

	private void Awake()
	{
		World = Instantiate(worldPrefab);
		clock = Instantiate(clockPrefab);
		crayonPath = Instantiate(crayonPathPrefab);
		player = Instantiate(playerPrefab);

		World.SetPlayer(player);
		player.SetDisplays(Instantiate(healthDisplayPrefab), Instantiate(statsDisplayPrefab), Instantiate(xpDisplayPrefab));
	}

	private void Start()
	{
		World.Regenerate();
	}

	// Update is called once per frame
	void Update()
	{
		if (gameEnded || player.IsMoving) return;

		// On tile hover
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitTile, 20, 1 << 6) &&
			hitTile.transform.TryGetComponent(out Tile tile))
		{
			// On tile click
			if (Input.GetMouseButtonDown(0))
			{
				player.SetPath(new(path.Reverse()));
				player.SetTargetedEnemy(hoveredEnemy);
			}

			// Check if hit enemy
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitEnemy, 20, 1 << 7) &&
				hitEnemy.transform.TryGetComponent(out UnitEnemy enemy) &&
				!enemy.IsDead &&
				World.GetNodeFromWorldPosition(enemy.transform.position) == tile.Node)
			{
				// Find best path
				int lowestCost = 10000;
				Stack<Node> lowestCostPath = null;
				for (int i = 0; i < 4; i++)
				{
					// Cast to ITuple to iterate through
					if ((World.GetNodeFromWorldPosition(enemy.transform.position).Neighbours as ITuple)[i] is not Node enemyNeighbourNode) continue;

					Stack<Node> comparisonPath = Pathfinding.Pathfind(player.DestinationNode, enemyNeighbourNode);
					if (comparisonPath == null) continue;

					int pathCost = Pathfinding.GetCost(comparisonPath);
					if (pathCost < lowestCost)
					{
						lowestCost = pathCost;
						lowestCostPath = comparisonPath;
					}
				}

				path = lowestCostPath ?? path;
				PreviewPath(path);

				hoveredEnemy = enemy;
			}
			else
			{
				player.SetTargetedEnemy(null);

				path = Pathfinding.Pathfind(player.DestinationNode, tile.Node) ?? path;
				PreviewPath(path);
			}
		}
	}

	void PreviewPath(Stack<Node> path)
	{
		crayonPath.Display(path);
		clock.ShowPreview(Pathfinding.GetCost(path));
	}

	public void Victory()
	{
		gameEnded = true;
		GameManager.Instance.Save.endlessModeUnlocked = true;

		if (InEndlessMode)
		{
			if (player.Level > GameManager.Instance.Save.bestScoreEndlessLevel)
			{
				GameManager.Instance.Save.bestScoreEndlessLevel = World.Generations;
			}
			else
			{

			}
		}
		else
		{
			if (World.Generations < GameManager.Instance.Save.bestScoreNormalGeneration)
			{
				GameManager.Instance.Save.bestScoreNormalGeneration = World.Generations;
			}
			else
			{

			}
		}

		_ = ReadWrite.Write(GameManager.Instance.Save);
	}

	public void GameOver()
	{
		gameEnded = true;
	}
}
