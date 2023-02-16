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

	public abstract void Trigger(Unit u);
}
