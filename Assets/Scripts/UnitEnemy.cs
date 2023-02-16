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
		UnitPlayer p = GameManager.Instance.player;
		sprite.flipX = p.transform.position.x > transform.position.x || p.transform.position.x >= transform.position.x && sprite.flipX;
		if (!IsDead)
		{
			anim.SetTrigger("attack");
		}
	}

	public void AttackPlayer() => AttackUnit(GameManager.Instance.player);

	public int XpToGive => Mathf.FloorToInt(Level * 3f);

	protected override void OnDeath()
	{
		unitUI.Deactivate();
		GameManager.Instance.player.GainXP(XpToGive);
		location.SetWalkable(true);
		anim.SetTrigger("death");
	}


	/// <summary>
	/// Damages the unit
	/// </summary>
	/// <param name="damageAmount">The amount to damage the user</param>
	public override void TakeDamage(int damageAmount)
	{
		UnitPlayer p = GameManager.Instance.player;
		sprite.flipX = p.transform.position.x > transform.position.x || p.transform.position.x >= transform.position.x && sprite.flipX;
		if (damageAmount <= 0)
		{
			Counterattack();
			return;
		}

		base.TakeDamage(damageAmount);
	}
}
