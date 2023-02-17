using UnityEngine;
using UnityEngine.UI;

public class DoomsdayClock : MonoBehaviour
{

	[field: SerializeField]
	public int Actions { get; private set; }

	[SerializeField]
	Image previewBar;
	[SerializeField]
	Image actualBar;

	float remaining = 1;

	public void ShowPreview(int cost)
	{
		actualBar.fillAmount = remaining - ((float)cost / Actions);
	}

	public bool Consume(int amount)
	{
		remaining = Mathf.Max(0, remaining - ((float)amount / Actions));
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;

		if (remaining == 0)
		{
			GameManager.Instance.GenerateNewWorld();
			ResetBar();
			return true;
		}
		return false;
	}

	public void ResetBar()
	{
		remaining = 1;
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;
	}
}
