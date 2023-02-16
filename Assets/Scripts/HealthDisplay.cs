using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	List<Image> hearts = new();
	[SerializeField]
	RectTransform container;
	[SerializeField]
	Sprite heartFullSprite;
	[SerializeField]
	Sprite heartEmptySprite;
	[SerializeField]
	Image heartPrefab;
	public void UpdateHearts(int health, int maxHealth)
	{
		while (hearts.Count < maxHealth)
		{
			Image h = Instantiate(heartPrefab, container);
			h.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
			hearts.Add(h);
		}
		for (int i = 0; i < hearts.Count; i++)
		{
			hearts[i].sprite = i + 1 <= health ? heartFullSprite : heartEmptySprite;
		}
	}
}
