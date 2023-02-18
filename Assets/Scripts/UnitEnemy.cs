using UnityEngine;

public class UnitEnemy : Unit
{
	Tile location;
	UnitPlayer player;

	public void Init(UnitPlayer p, Tile loc)
	{
		player = p;
		SetLocation(loc);
		location.Node.SetWalkable(false);
		SetLevelAndStats(Mathf.Max(1, player.Level + Random.Range(-1, 2)));
	}

	public void SetLocation(Tile tile)
	{
		location = tile;

		OffenceModifier = tile.OffenceModifier;
		DefenceModifier = tile.DefenceModifier;
		unitUI.UpdateText(this);
	}


	public void Counterattack()
	{
		sprite.flipX = player.transform.position.x > transform.position.x || (player.transform.position.x >= transform.position.x && sprite.flipX);
		if (!IsDead)
		{
			anim.SetTrigger("attack");
		}
	}

	public void AttackPlayer() => AttackUnit(player);

	public int XpToGive => Mathf.FloorToInt(Level * 3f);

	protected override void OnDeath()
	{
		unitUI.Deactivate();
		player.GainXP(XpToGive);
		location.Node.SetWalkable(true);
		anim.SetTrigger("death");
	}


	/// <summary>
	/// Damages the unit
	/// </summary>
	/// <param name="damageAmount">The amount to damage the user</param>
	public override void TakeDamage(int damageAmount)
	{
		sprite.flipX = player.transform.position.x > transform.position.x || (player.transform.position.x >= transform.position.x && sprite.flipX);
		if (damageAmount <= 0)
		{
			Counterattack();
			return;
		}

		base.TakeDamage(damageAmount);
	}
}
