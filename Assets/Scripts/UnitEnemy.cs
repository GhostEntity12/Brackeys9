using UnityEngine;

public class UnitEnemy : Unit
{
	Node location;

	public void SetLocation(Node node, TileBase tile)
	{
		location = node;

		OffenceModifier = tile.OffenceModifier;
		DefenceModifier = tile.DefenceModifier;
		unitUI.UpdateText(this);
	}


	public void Counterattack()
	{
		if (!IsDead)
		{
			anim.SetTrigger("attack");
		}
	}

	public void AttackPlayer() => AttackUnit(GameManager.Instance.player);

	public int XpToGive => Mathf.FloorToInt(Level * 3f);

	protected override void OnDeath()
	{
		GameManager.Instance.player.GainXP(XpToGive);
		location.SetWalkable(true);
		// Kill enemy - TODO: Change sprite instead
		anim.SetTrigger("death");
	}


	/// <summary>
	/// Damages the unit
	/// </summary>
	/// <param name="damageAmount">The amount to damage the user</param>
	public override void TakeDamage(int damageAmount)
	{
		if (damageAmount <= 0)
		{
			Counterattack();
			return;
		}

		Health = Mathf.Max(0, Health - damageAmount);
		if (Health == 0)
		{
			OnDeath();
			IsDead = true;
		}
		unitUI.UpdateText(this);

		anim.SetTrigger("hurt");
	}
}
