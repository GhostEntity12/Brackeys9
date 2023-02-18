using UnityEngine;

public class PlayerItemGet : MonoBehaviour
{
	public enum Item { Sword, Shield }

	float timer = 0;
	bool active = false;
	SpriteRenderer spriteRenderer;
	[SerializeField]
	Sprite[] itemSprites;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!active) return;

		timer += Time.deltaTime;
		if (timer <= 0.75f) return;

		timer = 0;
		spriteRenderer.enabled = false;
		active = false;
	}

	public void Activate(Item item)
	{
		active = true;
		spriteRenderer.sprite = itemSprites[(int)item];
		spriteRenderer.enabled = true;
	}
}
