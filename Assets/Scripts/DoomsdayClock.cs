using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomsdayClock : MonoBehaviour
{
	[SerializeField]
	Image previewBar;
	[SerializeField]
	Image actualBar;

	float remaining = 1;

	public void ShowPreview(int cost)
	{
		actualBar.fillAmount = remaining - ((float)cost / GameManager.Instance.Actions);
	}

	public void Consume(int amount)
	{
		remaining = Mathf.Max(0, remaining - ((float)amount / GameManager.Instance.Actions));
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;

		if (remaining == 0)
		{
			GameManager.Instance.GenerateNewWorld();
		}
	}

	public void ResetBar()
	{
		remaining = 1;
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;
	}
}
