using UnityEngine;

public class UnitEnemy : Unit
{
	public int xpToGive => Mathf.FloorToInt(level * 2.5f);

	private void Awake()
	{
		AssignStats();
	}

	protected override void OnDeath()
	{
		GameManager.Instance.player.GainXP(xpToGive);
		// Kill enemy
	}
}
