using UnityEngine;

public abstract class Unit : MonoBehaviour
{


	protected bool isDead = false;

	//Properties
	[Header("Stats")]
	protected int level = 1;
	[field: SerializeField]
	public int MaxHealth { get; protected set; } = 1;
	[field: SerializeField]
	public int Health { get; protected set; }
	[field: SerializeField]
	public int Offence { get; protected set; }
	[field: SerializeField]
	public int Defence { get; protected set; }

	public int CalculatedOffence => Offence + offenceModifier;
	public int CalculatedDefence => Defence + defenceModifier;

	protected int offenceModifier = 0;
	protected int defenceModifier = 0;

	public UnitUI unitUI;


	/// <summary>
	/// Attacks a given unit
	/// </summary>
	/// <param name="target">The unit this unit should attack</param>
	public void AttackUnit(Unit target)
	{
		int damageToDeal = Mathf.Max(0, CalculatedOffence - target.CalculatedDefence);

		target.TakeDamage(damageToDeal);
		if (!target.isDead)
		{
			target.AttackUnit(this);
		}
	}

	/// <summary>
	/// Heals damage done to the unit
	/// </summary>
	/// <param name="healAmount">The amount of health to recover</param>
	public void HealDamage(int healAmount) => Health = Mathf.Min(MaxHealth, Health + healAmount);

	/// <summary>
	/// Damages the unit
	/// </summary>
	/// <param name="damageAmount">The amount to damage the user</param>
	public void TakeDamage(int damageAmount)
	{
		Health = Mathf.Max(0, Health - damageAmount);
		if (Health == 0)
		{
			OnDeath();
			isDead = true;
		}
	}

	protected abstract void OnDeath();

	/// <summary>
	/// Resets the stat modifiers on the unit
	/// </summary>
	public void ResetModifiers()
	{
		offenceModifier = 0;
		defenceModifier = 0;
	}

	/// <summary>
	/// Randomly assigns stats
	/// </summary>
	public void AssignStats()
	{
		for (int i = 0; i < level + 5; i++)
		{
			switch (Random.Range(0, 3))
			{
				case 0:
					MaxHealth++;
					break;
				case 1:
					Offence++;
					break; ;
				case 2:
					Defence++;
					break;
				default:
					break;
			}
		}
		Health = MaxHealth;
		unitUI.UpdateText(Health, MaxHealth, Offence, Defence);
	}
}
