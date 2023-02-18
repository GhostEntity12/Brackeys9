using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPDisplay : MonoBehaviour
{
	[SerializeField]
	Image fill;
	[SerializeField]
	TextMeshProUGUI levelText;
	public void UpdateXP(float amount) => fill.fillAmount = amount;

	public void UpdateLevelText(int newLevel) => levelText.text = $"Level {newLevel}";
}
