using UnityEngine;

public class UnitEnemy : Unit
{
	Node location;

	public void SetLocation(Node node, TileBase tile)
	{
		location = node;

		offenceModifier = tile.OffenceModifier;
		defenceModifier = tile.DefenceModifier;
	}

	public int XpToGive => Mathf.FloorToInt(level * 3f);

	private void Awake()
	{
		AssignStats();
	}

	protected override void OnDeath()
	{
		GameManager.Instance.player.GainXP(XpToGive);
		location.SetWalkable(true);
		// Kill enemy - TODO: Change sprite instead
		Destroy(gameObject);
	}

	public override void TakeDamage(int amount)
	{
		base.TakeDamage(amount);
		if (!isDead)
		{
			AttackUnit(GameManager.Instance.player);
		}
	}
}
