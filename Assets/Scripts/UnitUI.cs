using TMPro;
using UnityEngine;

[System.Serializable]
public class UnitUI
{
	
	public TextMeshProUGUI offenceText;
	public TextMeshProUGUI defenceText;
	public TextMeshProUGUI healthText;

	public void UpdateText(int health, int maxHealth, int offence, int defence)
	{
		healthText.text = $"{health}/{maxHealth}";
		offenceText.text = $"{offence}";
		defenceText.text = $"{defence}";
	}
}
