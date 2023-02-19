using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	readonly List<Image> hearts = new();
	[SerializeField]
	RectTransform container;
	[SerializeField]
	Image heartPrefab;
	[SerializeField]
	Image plusPrefab;
	[SerializeField]
	Sprite heartFullSprite;
	[SerializeField]
	Sprite heartEmptySprite;

	const int MaxHearts = 11;

	public void UpdateHearts(int health, int maxHealth)
	{
		while (container.childCount < maxHealth && container.childCount <= MaxHearts)
		{
			SpawnNewHeart();
		}
		for (int i = 0; i < hearts.Count; i++)
		{
			hearts[i].sprite = i + 1 <= health ? heartFullSprite : heartEmptySprite;
		}
	}

	void SpawnNewHeart()
	{
		switch (hearts.Count)
		{
			case < MaxHearts:
				Image h = Instantiate(heartPrefab, container);
				h.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
				hearts.Add(h);
				break;
			case MaxHearts:
				Instantiate(plusPrefab, container);
				break;
			default:
				break;
		}
	}
}
