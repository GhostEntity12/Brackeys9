using TMPro;
using UnityEngine;

[System.Serializable]
public class UnitUI
{

	public TextMeshProUGUI offenceText;
	public TextMeshProUGUI defenceText;
	public TextMeshProUGUI healthText;

	public void UpdateText(Unit u)//int health, int maxHealth, int offence, int offenceMod, int defence, int defenceMod)
	{
		healthText.text = $"{u.Health}/{u.MaxHealth}";
		offenceText.text = $"{Mathf.Max(0, u.Offence + u.OffenceModifier)}";
		offenceText.color = TextColorModifier(u.OffenceModifier);
		defenceText.text = $"{Mathf.Max(0, u.Defence + u.DefenceModifier)}";
		defenceText.color = TextColorModifier(u.DefenceModifier);
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
