using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPDisplay : MonoBehaviour
{
	[SerializeField]
	Image fill;
	[SerializeField]
	TextMeshProUGUI levelText;
	[SerializeField]
	SpriteRenderer goal;


	private void Start()
	{
		if (SceneLoadTypeData.GetInstance().loadType == SceneLoadTypeData.LoadType.Endless)
		{
			goal.enabled = false;
		}
	}

	public void UpdateXP(float amount) => fill.fillAmount = amount;

	public void UpdateLevelText(int newLevel) => levelText.text = $"{newLevel}";
}
