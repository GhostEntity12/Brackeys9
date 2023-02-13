using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class World : MonoBehaviour
{
	// NOTE: Nodes are traversal objects, tiles are GameObjects
	public enum Direction { Left, Right, Up, Down }
	const int WorldXSize = 11;
	const int WorldYSize = 11;

	// The different types of tile that can be spawned
	public Tile[] spawnableTilePrefabs;

	// The array of tiles in the world
	Tile[,] worldTiles;

	// Start is called before the first frame update
	void Start()
	{
		Generate();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void Generate()
	{
		worldTiles = new Tile[WorldXSize, WorldYSize];
		// Spawn tiles
		GameObject world = new GameObject("World");
		for (int y = 0; y < WorldYSize; y++)
		{
			GameObject row = new GameObject($"Row {y}");
			row.transform.parent = world.transform;
			for (int x = 0; x < WorldXSize; x++)
			{
				Tile t = Instantiate(spawnableTilePrefabs[Random.Range(0, spawnableTilePrefabs.Length)], new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(90, 0, 0));
				t.node.Init(x, y, t.movementCost);
				worldTiles[x, y] = t;
				t.name = $"{x}, {y}";
				t.transform.parent = row.transform;
			}
		}
	}

	void Pathfind(Node from, Node to)
	{
		PriorityQueue<Node, int> tilesToVisit = new();
		tilesToVisit.Enqueue(from, 0);
	}

	public Node AdjacentNode(Direction direction, Node sourceNode)
	{
		switch (direction)
		{
			case Direction.Left:
				return sourceNode.xPos - 1 < 0 ? null : worldTiles[sourceNode.xPos - 1, sourceNode.yPos].node;
			case Direction.Right:
				return sourceNode.xPos + 1 > WorldXSize ? null : worldTiles[sourceNode.xPos + 1, sourceNode.yPos].node;
			case Direction.Up:
				return sourceNode.yPos + 1 > WorldYSize ? null : worldTiles[sourceNode.xPos, sourceNode.yPos + 1].node;
			case Direction.Down:
				return sourceNode.yPos - 1 < 0 ? null : worldTiles[sourceNode.xPos, sourceNode.yPos - 1].node;
			default:
				return null;
		}
	}
}
