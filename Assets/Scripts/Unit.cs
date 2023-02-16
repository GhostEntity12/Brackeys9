using TMPro;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{

	public bool IsDead { get; protected set; } = false;
	[field: SerializeField]
	public int Level { get; protected set; }
	[field: SerializeField]
	public int MaxHealth { get; protected set; } = 1;
	[field: SerializeField]
	public int Health { get; protected set; }
	[field: SerializeField]
	public int Offence { get; protected set; }
	[field: SerializeField]
	public int OffenceModifier { get; protected set; } = 0;
	[field: SerializeField]
	public int Defence { get; protected set; }
	[field: SerializeField]
	public int DefenceModifier { get; protected set; } = 0;

	public int CalculatedOffence => Mathf.Max(0, Offence + OffenceModifier);
	public int CalculatedDefence => Mathf.Max(0, Defence + DefenceModifier);

	protected Animator anim;
	protected SpriteRenderer sprite;

	[SerializeField]
	DamageNumber dn;

	[SerializeField]
	protected UnitUI unitUI;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
	}

	public void SetLevel(int level) => Level = level;

	/// <summary>
	/// Attacks a given unit
	/// </summary>
	/// <param name="target">The unit this unit should attack</param>
	public virtual void AttackUnit(Unit target)
	{
		if (target != null)
		{
			target.TakeDamage(Mathf.Max(0, CalculatedOffence - target.CalculatedDefence));
		}
	}

	/// <summary>
	/// Heals damage done to the unit
	/// </summary>
	/// <param name="healAmount">The amount of health to recover</param>
	public virtual void HealDamage(int healAmount)
	{
		Health = Mathf.Min(MaxHealth, Health + healAmount);
	}

	/// <summary>
	/// Damages the unit
	/// </summary>
	/// <param name="damageAmount">The amount to damage the user</param>
	public virtual void TakeDamage(int damageAmount)
	{
		if (damageAmount <= 0) return;

		Health = Mathf.Max(0, Health - damageAmount);
		if (Health == 0)
		{
			OnDeath();
			IsDead = true;
		}
		unitUI.UpdateText(this);

		anim.SetTrigger("hurt");

		dn.Trigger(damageAmount);
	}

	protected abstract void OnDeath();

	/// <summary>
	/// Resets the stat modifiers on the unit
	/// </summary>
	public void ResetModifiers()
	{
		OffenceModifier = 0;
		DefenceModifier = 0;
	}

	/// <summary>
	/// Randomly assigns stats
	/// </summary>
	public void AssignStats(int numberOfStats)
	{
		for (int i = 0; i < numberOfStats; i++)
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
		unitUI.UpdateText(this);
	}
}
