using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

public class World : MonoBehaviour
{
	// NOTE: Nodes are traversal objects, tiles are GameObjects
	public enum Direction { Left, Right, Up, Down }
	const int WorldXSize = 11;
	const int WorldYSize = 11;

	// The prefabs of the tiles that can be spawned
	public Tile[] spawnableTilePrefabs;

	// The prefab of enemy to spawn
	public UnitEnemy enemyPrefab;

	// The array of tiles in the world
	Node[,] worldNodes;

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
		for (int y = 0; y < WorldYSize; y++)
		{
			for (int x = 0; x < WorldXSize; x++)
			{
				Node n = worldNodes[x, y];
				n.SetNeighbours(GetAdjacentNode(Direction.Left, n), GetAdjacentNode(Direction.Right, n), GetAdjacentNode(Direction.Up, n), GetAdjacentNode(Direction.Down, n));
			}
		}

		// Spawn enemies
		for (int i = 0; i < Random.Range(enemiesToSpawn.x, enemiesToSpawn.y); i++)
		{

		}
	}

	public Node GetNodeFromWorldPosition(Vector3 pos) => (pos.x >= WorldXSize || pos.z >= WorldYSize) ? null : worldNodes[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)];

	public Node GetAdjacentNode(Direction direction, Node sourceNode) => GetAdjacentNode(direction, sourceNode, worldNodes);
	Node GetAdjacentNode(Direction direction, Node sourceNode, Node[,] allNodes)
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
			int temp = enemiesToSpawn.y;
			enemiesToSpawn.y = enemiesToSpawn.x;
			enemiesToSpawn.x = temp;
		}
	}
}