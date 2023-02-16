using UnityEngine;

public abstract class TileBase : MonoBehaviour
{
	[field: SerializeField]
	public int MovementCost { get; protected set; }
	[field: SerializeField]
	public bool IsWalkable { get; protected set; }

	public Node Node { get; private set; }

	public int OffenceModifier { get; protected set; }
	public int DefenceModifier { get; protected set; }

	public abstract void Trigger();
}
