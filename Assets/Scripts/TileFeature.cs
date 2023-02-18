using UnityEngine;

public abstract class TileFeature : MonoBehaviour
{
	new SpriteRenderer renderer;
	[SerializeField]
	Sprite activeSprite;
	[SerializeField]
	Sprite inactiveSprite;
	bool triggered;

	private void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = activeSprite;
	}
	public virtual bool Trigger(Unit u)
	{
		if (triggered) return false;
		triggered = true;
		renderer.sprite = inactiveSprite;
		return true;
	}

	public void ResetFeature()
	{
		triggered = false;
		renderer.sprite = activeSprite;
	}
}
