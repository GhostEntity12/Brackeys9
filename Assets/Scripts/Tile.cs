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

	public void DestroyTile(float delay)
	{
		LeanTween.scale(gameObject, Vector3.zero, 0.3f).setDelay(delay).setEase(LeanTweenType.easeInBack).setDestroyOnComplete(true);
	}
	public void SpawnTile(float delay)
	{
		LeanTween.scale(gameObject, Vector3.one, 0.3f).setDelay(delay).setEase(LeanTweenType.easeOutBack);
	}
}
