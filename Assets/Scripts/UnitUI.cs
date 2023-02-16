using TMPro;
using UnityEngine;

[System.Serializable]
public class UnitUI
{

	public TextMeshProUGUI offenceText;
	public TextMeshProUGUI defenceText;
	public TextMeshProUGUI healthText;

	public void UpdateText(int health, int maxHealth, int offence, int offenceMod, int defence, int defenceMod)
	{
		healthText.text = $"{health}/{maxHealth}";
		offenceText.text = $"{Mathf.Max(0, offence + offenceMod)}";
		offenceText.color = TextColorModifier(offenceMod);
		defenceText.text = $"{Mathf.Max(0, defence + defenceMod)}";
		defenceText.color = TextColorModifier(defenceMod);
	}

	Color TextColorModifier(int modifier)
	{
		return modifier switch
		{
			> 0 => Color.green,
			< 0 => Color.red,
			_ => Color.white
		};
	}
}
