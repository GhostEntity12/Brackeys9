using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class World : MonoBehaviour
{
	// NOTE: Nodes are traversal objects, tiles are GameObjects
	public enum Direction { Left, Right, Up, Down }
	const int WorldXSize = 11;
	const int WorldYSize = 11;

	// The prefabs of the tiles that can be spawned
	public TileBase[] spawnableTilePrefabs;

	// The prefab of enemy to spawn
	public UnitEnemy enemyPrefab;

	// The array of nodes in the world
	Node[,] worldNodes;
	// The array of tiles in the world
	TileBase[,] worldTiles;

	[SerializeField]
	Vector2Int enemiesToSpawn = new(3, 5);
	// Start is called before the first frame update
	void Start()
	{
		Generate();
	}

	void Generate()
	{
		worldNodes = new Node[WorldXSize, WorldYSize];
		worldTiles = new TileBase[WorldXSize, WorldYSize];

		// Spawning tiles
		GameObject world = new("World");
		for (int y = 0; y < WorldYSize; y++)
		{
			GameObject row = new($"Row {y}");
			row.transform.parent = world.transform;
			for (int x = 0; x < WorldXSize; x++)
			{
				// Instantiating tile
				TileBase t = Instantiate(spawnableTilePrefabs[Random.Range(0, spawnableTilePrefabs.Length)], new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(90, 0, 0));
				// Saving to arrays
				worldNodes[x, y] = t.Node;
				worldTiles[x, y] = t;
				// Initializing and setting parent
				t.Node.Init(x, y, t.MovementCost, t.IsWalkable);
				t.name = $"{x}, {y}";
				t.transform.parent = row.transform;
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

		// Spawn enemies
		for (int i = 0; i < Random.Range(enemiesToSpawn.x, enemiesToSpawn.y); i++)
		{
			// 100 attempts per enemy
			int maxAttempts = 100;
			int attempts = 0;
			do
			{
				attempts++;
				Vector2Int placePosition = new(Random.Range(0, WorldXSize), Random.Range(0, WorldYSize));
				Node enemyPlaceAttemptNode = worldNodes[placePosition.x, placePosition.y];
				// Disallow spawning on unwalkable tiles
				if (enemyPlaceAttemptNode.IsWalkable)
				{
					Instantiate(enemyPrefab, new(enemyPlaceAttemptNode.XPos + 0.5f, 0.2f, enemyPlaceAttemptNode.YPos + 0.5f), Quaternion.identity).SetLocation(enemyPlaceAttemptNode, worldTiles[placePosition.x, placePosition.y]);
					// Prevent the player from walking through enemies
					enemyPlaceAttemptNode.SetWalkable(false);
					break;
				}

			} while (attempts < maxAttempts);
		}
	}

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
	public TileBase GetTile(int x, int y) => (x >= WorldXSize || x < 0 || y >= WorldYSize || y < 0) ? null : worldTiles[x, y];

	/// <summary>
	/// Returns a node based on world position. Returns null if out of range
	/// </summary>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	/// <returns>The node at x, y</returns>
	public Node GetNodeFromWorldPosition(Vector3 pos) => (pos.x >= WorldXSize || pos.x < 0 || pos.z >= WorldYSize || pos.z < 0) ? null : worldNodes[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)];

	/// <summary>
	/// Returns the adacent node in a direction of a given node
	/// </summary>
	/// <param name="sourceNode">The node to be checked</param>
	/// <param name="direction">Which direction's node should be returned</param>
	/// <returns></returns>
	public Node GetAdjacentNode(Node sourceNode, Direction direction) => GetAdjacentNode(sourceNode, direction, worldNodes);
	Node GetAdjacentNode(Node sourceNode, Direction direction, Node[,] allNodes)
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

	private void OnValidate()
	{
		if (enemiesToSpawn.y < enemiesToSpawn.x)
		{
			(enemiesToSpawn.x, enemiesToSpawn.y) = (enemiesToSpawn.y, enemiesToSpawn.x);
		}
	}
}