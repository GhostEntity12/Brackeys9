using UnityEngine;
using UnityEngine.UI;

public class DoomsdayClock : MonoBehaviour
{
	LevelManager levelManager;

	[field: SerializeField]
	public int Actions { get; private set; }

	[SerializeField]
	Image previewBar;
	[SerializeField]
	Image actualBar;

	float remaining = 1;

	public bool HasTimeRemaining => remaining > 0.001;

	private void Awake()
	{
		levelManager = FindObjectOfType<LevelManager>();
	}

	public void ShowPreview(int cost) => actualBar.fillAmount = remaining - ((float)cost / Actions);

	public void Consume(int amount)
	{
		remaining = Mathf.Max(0, remaining - ((float)amount / Actions));
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;
	}

	public bool CheckRegenerate()
	{
		if (remaining >= 0.001) return false;

		ResetValues();
		levelManager.World.Regenerate();
		return true;
	}

	public void ResetValues()
	{
		remaining = 1;
		actualBar.fillAmount = remaining;
		previewBar.fillAmount = remaining;
	}
}
