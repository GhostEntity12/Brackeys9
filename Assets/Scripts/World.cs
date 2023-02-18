using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	UnitPlayer player;
	LevelManager levelManager;

	// NOTE: Nodes are traversal objects, tiles are GameObjects
	public enum Direction { Left, Right, Up, Down }
	const int WorldXSize = 11;
	const int WorldYSize = 11;

	/// <summary>
	/// The array of nodes in the world
	/// </summary>
	Node[,] worldNodes;
	/// <summary>
	/// The array of tiles in the world
	/// </summary>
	Tile[,] worldTiles;

	[Header("Tiles")]
	[SerializeField]
	Tile[] spawnableTilePrefabs;
	[SerializeField]
	float[] spawnProbabilities;

	/// <summary>
	/// The spawnable prefabs 
	/// </summary>
	[Header("Prefabs")]
	[SerializeField]
	UnitEnemy enemyPrefab;
	[SerializeField] TileFeature chestPrefab;
	[SerializeField]
	TileFeature farmPrefab;
	[SerializeField]
	TileFeature townPrefab;
	[SerializeField]
	TileFeature altarBlessedPrefab;
	[SerializeField]
	TileFeature altarCursedPrefab;

	[Header("Feature Chances")]
	[SerializeField]
	Vector2Int enemiesToSpawn = new(3, 5);
	[SerializeField]
	Vector2Int townsToSpawn = new(0, 2);
	[SerializeField]
	Vector2Int farmsToSpawn = new(2, 5);
	[SerializeField]
	float chestSpawnChance = 0.3f;
	[SerializeField]
	float altarBlessedSpawnChance = 0.4f;
	[SerializeField]
	float altarCursedSpawnChance = 0.4f;

	readonly List<UnitEnemy> enemies = new();
	readonly List<TileFeature> features = new();

	// Containers (used for destruction of the world)
	GameObject world;
	GameObject featureContainer;
	GameObject enemyContainer;

	int queuedChests = 1;

	public int Generations { get; private set; }


	/// <summary>
	/// Destroys the previous world and generates a new one
	/// </summary>
	public void Regenerate()
	{
		Destroy();
		LeanTween.delayedCall(1.4f, Generate);
	}

	/// <summary>
	/// Generates a new world
	/// </summary>
	public void Generate()
	{
		Generations++;

		// Prepare arrays
		worldNodes = new Node[WorldXSize, WorldYSize];
		worldTiles = new Tile[WorldXSize, WorldYSize];

		// Spawning tiles
		Destroy(world);
		world = new("World");
		for (int y = 0; y < WorldYSize; y++)
		{
			GameObject row = new($"Row {y}");
			row.transform.parent = world.transform;
			for (int x = 0; x < WorldXSize; x++)
			{
				// Instantiating tile
				Tile t = Instantiate(RandomTilePrefab(), new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(90, 0, 0));
				// Initializing and setting parent
				t.Node.Init(x, y, t.MovementCost, t.IsWalkable);
				t.name = $"{x}, {y}";
				t.transform.parent = row.transform;
				// Saving to arrays
				worldNodes[x, y] = t.Node;
				worldTiles[x, y] = t;
				t.SpawnTile(Random.value / 3f);
			}
		}
		// Setting neighbours
		for (int y = 0; y < WorldYSize; y++)
		{
			for (int x = 0; x < WorldXSize; x++)
			{
				Node n = worldNodes[x, y];
				n.SetNeighbours(GetAdjacentNode(n, Direction.Left), GetAdjacentNode(n, Direction.Right), GetAdjacentNode(n, Direction.Up), GetAdjacentNode(n, Direction.Down));
			}
		}

		// Spawn extras
		enemyContainer = new("Enemy");
		featureContainer = new("Features");
		SpawnEnemies();
		SpawnChests();
		SpawnFeatures(farmsToSpawn, farmPrefab);
		SpawnFeatures(townsToSpawn, townPrefab);
		SpawnFeatures(altarBlessedSpawnChance, altarBlessedPrefab);
		SpawnFeatures(altarCursedSpawnChance, altarCursedPrefab);

		LeanTween.delayedCall(0.5f, () => player.ResetNode());
		LeanTween.delayedCall(0.5f, () => levelManager.ToggleAllowInput(true));
	}

	/// <summary>
	/// Destroys the world
	/// </summary>
	public void Destroy()
	{
		levelManager.ToggleAllowInput(false);
		enemies.Clear();
		features.Clear();

		for (int y = 0; y < WorldYSize; y++)
		{
			for (int x = 0; x < WorldXSize; x++)
			{
				worldTiles[x, y].DestroyTile(Random.value);
			}
		}
		Destroy(enemyContainer);
		Destroy(featureContainer);
	}

	/// <summary>
	/// Returns a random tile prefab
	/// </summary>
	/// <returns></returns>
	Tile RandomTilePrefab()
	{
		float rand = Random.value;

		for (int i = 0; i < spawnProbabilities.Length; i++)
		{
			float probability = spawnProbabilities[i];
			if (rand > probability)
			{
				rand -= probability;
			}
			else
			{
				return spawnableTilePrefabs[i];
			}
		}
		return spawnableTilePrefabs[^1];
	}

	void SpawnEnemies()
	{
		// Cache player node
		Node playerNode = GetNodeFromWorldPosition(player.transform.position);

		for (int i = 0; i < Random.Range(enemiesToSpawn.x, enemiesToSpawn.y); i++)
		{
			// 100 attempts per enemy
			int maxAttempts = 100;
			int attempts = 0;
			do
			{
				attempts++;
				// Random tile
				Vector2Int placePosition = new(Random.Range(0, WorldXSize), Random.Range(0, WorldYSize));
				Node enemyPlaceAttemptNode = worldNodes[placePosition.x, placePosition.y];

				// Disallow spawning on unwalkable tiles or player's tile
				if (enemyPlaceAttemptNode.IsWalkable && enemyPlaceAttemptNode != playerNode)
				{
					UnitEnemy enemy = Instantiate(enemyPrefab, new(enemyPlaceAttemptNode.XPos + 0.5f, 0.2f, enemyPlaceAttemptNode.YPos + 0.5f), Quaternion.identity);
					enemies.Add(enemy);
					enemy.transform.SetParent(enemyContainer.transform);
					enemy.Init(player, worldTiles[placePosition.x, placePosition.y]);
					break;
				}

			} while (attempts < maxAttempts);
		}
	}
	void SpawnChests()
	{
		if (queuedChests == 0) return;

		if (Random.value < chestSpawnChance)
		{
			queuedChests--;
			Vector2Int placePosition = new(Random.Range(0, WorldXSize), Random.Range(0, WorldYSize));
			Tile featureAttemptTile = worldTiles[placePosition.x, placePosition.y];
			// Disallow spawning on unwalkable tiles
			if (!featureAttemptTile.Node.IsWalkable || featureAttemptTile.Feature) return;
			SpawnFeature(chestPrefab, featureAttemptTile);
		}
	}
	void SpawnFeatures(Vector2 chanceRange, TileFeature featurePrefab)
	{
		for (int i = 0; i < Random.Range(chanceRange.x, chanceRange.y); i++)
		{
			// 100 attempts per feature
			int maxAttempts = 100;
			int attempts = 0;
			do
			{
				attempts++;
				Vector2Int placePosition = new(Random.Range(0, WorldXSize), Random.Range(0, WorldYSize));
				Tile featureAttemptTile = worldTiles[placePosition.x, placePosition.y];

				// Disallow spawning on unwalkable tiles
				if (!featureAttemptTile.Node.IsWalkable || featureAttemptTile.Feature) continue;

				SpawnFeature(featurePrefab, featureAttemptTile);
				break;

			} while (attempts < maxAttempts);
		}
	}
	void SpawnFeatures(float chance, TileFeature featurePrefab)
	{
		if (Random.value < chance)
		{
			Vector2Int placePosition = new(Random.Range(0, WorldXSize), Random.Range(0, WorldYSize));
			Tile featureAttemptTile = worldTiles[placePosition.x, placePosition.y];

			// Disallow spawning on unwalkable tiles
			if (!featureAttemptTile.Node.IsWalkable || featureAttemptTile.Feature) return;

			SpawnFeature(featurePrefab, featureAttemptTile);
		}
	}
	void SpawnFeature(TileFeature prefab, Tile tile)
	{
		TileFeature feature = Instantiate(prefab, tile.transform.position + Vector3.up * 0.05f, Quaternion.Euler(90, 0, 0));
		features.Add(feature);
		tile.SetFeature(feature);
		feature.transform.SetParent(featureContainer.transform);
	}

	public void AddChestsToQueue(int count) => queuedChests += count;

	/// <summary>
	/// Returns a node based on x and y grid coordinates. Returns null if out of range
	/// </summary>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	/// <returns>The node at x, y</returns>
	public Node GetNode(int x, int y) => (x >= WorldXSize || x < 0 || y >= WorldYSize || y < 0) ? null : worldNodes[x, y];

	/// <summary>
	/// Returns a tile based on x and y grid coordinates. Returns null if out of range
	/// </summary>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	/// <returns>The tile at x, y</returns>
	public Tile GetTile(int x, int y) => (x >= WorldXSize || x < 0 || y >= WorldYSize || y < 0) ? null : worldTiles[x, y];

	/// <summary>
	/// Returns a node based on world position. Returns null if out of range
	/// </summary>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	/// <returns>The node at x, y</returns>
	public Node GetNodeFromWorldPosition(Vector3 pos) => (pos.x >= WorldXSize || pos.x < 0 || pos.z >= WorldYSize || pos.z < 0) ? null : worldNodes[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)];

	Node GetAdjacentNode(Node sourceNode, Direction direction)
	{
		return direction switch
		{
			Direction.Left => sourceNode.XPos - 1 < 0 ? null : worldNodes[sourceNode.XPos - 1, sourceNode.YPos],
			Direction.Right => sourceNode.XPos + 1 >= WorldXSize ? null : worldNodes[sourceNode.XPos + 1, sourceNode.YPos],
			Direction.Up => sourceNode.YPos + 1 >= WorldYSize ? null : worldNodes[sourceNode.XPos, sourceNode.YPos + 1],
			Direction.Down => sourceNode.YPos - 1 < 0 ? null : worldNodes[sourceNode.XPos, sourceNode.YPos - 1],
			_ => null,
		};
	}

	public void SetPlayer(UnitPlayer p) => player = p;
	public void SetLevelManager(LevelManager lm) => levelManager = lm;
	private void OnValidate()
	{
		if (enemiesToSpawn.y < enemiesToSpawn.x)
		{
			(enemiesToSpawn.x, enemiesToSpawn.y) = (enemiesToSpawn.y, enemiesToSpawn.x);
		}
	}
}