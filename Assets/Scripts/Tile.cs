using UnityEngine;

public class Tile : MonoBehaviour
{
	public Node Node { get; private set; } = new();

	[field: SerializeField]
	public bool IsWalkable { get; protected set; }
	[field: SerializeField]
	public int MovementCost { get; protected set; }

	[field: SerializeField]
	public int OffenceModifier { get; protected set; }
	[field: SerializeField]
	public int DefenceModifier { get; protected set; }

	public UnitEnemy Enemy { get; protected set; }
	public TileFeature Feature { get; protected set; }

	public void SetFeature(TileFeature feature) => Feature = feature;
}
