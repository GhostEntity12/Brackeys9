using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI offenceText;
	[SerializeField]
	TextMeshProUGUI defenceText;

	public void UpdateStats(Unit u)
	{
		offenceText.text = u.OffenceModifier switch
		{
			< 0 => $"– {u.Offence}<color=red>{u.OffenceModifier}",
			0 => $"– {u.Offence}",
			> 0 => $"– {u.Offence}<color=green>+{u.OffenceModifier}"
		};

		defenceText.text = u.DefenceModifier switch
		{
			< 0 => $"– {u.Defence}<color=red>{u.DefenceModifier}",
			0 => $"– {u.Defence}",
			> 0 => $"– {u.Defence}<color=green>+{u.DefenceModifier}"
		};
	}
}
