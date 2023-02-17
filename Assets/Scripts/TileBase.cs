using UnityEngine;

public abstract class TileBase : MonoBehaviour
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

	public void SetEnemy(UnitEnemy enemy) => Enemy = enemy;

	public void SetFeature(TileFeature feature) => Feature = feature;

	public void RemoveEnemy() => Enemy = null;
}
