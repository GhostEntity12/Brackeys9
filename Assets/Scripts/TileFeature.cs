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
		LeanTween.scale(gameObject, Vector3.one, 0.3f).setEaseOutCubic().setDelay(0.6f);
		LeanTween.value(gameObject, SetAlpha, 0, 1, 0.3f).setEaseOutCubic().setDelay(0.6f);
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

	void SetAlpha(float alpha)
	{
		renderer.color = new Color(1, 1, 1, alpha);
	}
}
