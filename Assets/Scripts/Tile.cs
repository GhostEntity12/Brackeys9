using UnityEngine;

public class Tile : MonoBehaviour
{
	[field: SerializeField]
	public int MovementCost { get; private set; }
	[field: SerializeField]
	public bool IsWalkable { get; private set; }

	public Tile(bool isWalkable)
	{
		IsWalkable = isWalkable;
	}

	public Node node;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}
}
