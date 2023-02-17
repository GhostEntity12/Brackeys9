using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : Unit
{
	const float LevelCurveStrength = 1.403f;
	const float TargetLevel = 10;

	[SerializeField]
	HealthDisplay healthDisplay;
	[SerializeField]
	StatsDisplay statsDisplay;
	[SerializeField]
	XPDisplay xpDisplay;

	[SerializeField]
	float yOffset = 0.2f;
	[SerializeField]
	float speed = 1f;
	[SerializeField]
	float allowedError = 0.01f;

	Stack<Node> path = new();
	Vector3 currentWaypoint;
	public Node DestinationNode { get; private set; }
	public bool IsMoving { get; private set; }

	UnitEnemy targetedEnemy = null;

	bool queueAttack;

	[SerializeField]
	PlayerItemGet itemGetSprite;

	int xp;
	int XpToNextLevel
	{
		get
		{
			if (Level <= TargetLevel)
			{
				return Mathf.FloorToInt(Mathf.Pow(LevelCurveStrength, Level - 1) + 9);
			}
			else
			{
				return Mathf.FloorToInt((Mathf.Pow(LevelCurveStrength, TargetLevel - 1) + 9) + (Level - TargetLevel) * 10);
			}
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		Offence = 2;
		Defence = 2;
		MaxHealth = 2;
		Health = MaxHealth;

		if (Random.value < 0.33f)
		{
			IncreaseDefence(1);
		}
		else if (Random.value < 0.66)
		{
			IncreaseOffence(1);
		}
		else
		{
			IncreaseMaxHealth(1);
			HealDamage(1);
		}

		healthDisplay.UpdateHearts(Health, MaxHealth);
		statsDisplay.UpdateStats(this);

		DestinationNode = GameManager.Instance.World.GetNodeFromWorldPosition(transform.position);
		currentWaypoint = new(DestinationNode.XPos + 0.5f, yOffset, DestinationNode.YPos + 0.5f);
		transform.position = transform.position + Vector3.up * yOffset;
	}

	// Update is called once per frame
	void Update()
	{
		anim.SetBool("isMoving", IsMoving);
		if (Vector3.Distance(transform.position, currentWaypoint) < allowedError)
		{
			if (path.Count > 0)
			{
				SetNextWaypoint();
			}
			else
			{
				IsMoving = false;
				// Activate tile related events (including stat changes)
				TileBase destinationTile = GameManager.Instance.World.GetTile(DestinationNode.XPos, DestinationNode.YPos);
				OffenceModifier = destinationTile.OffenceModifier;
				DefenceModifier = destinationTile.DefenceModifier;
				if (destinationTile.Feature)
				{
					destinationTile.Feature.Trigger(this);
				}
				unitUI.UpdateText(this);
				statsDisplay.UpdateStats(this);

				// Attack the target if set
				if (targetedEnemy && !queueAttack)
				{
					sprite.flipX = targetedEnemy.transform.position.x < transform.position.x || targetedEnemy.transform.position.x <= transform.position.x && sprite.flipX;
					anim.SetTrigger("attack");
					queueAttack = true;
				}
				return;
			}
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			sprite.flipX = currentWaypoint.x < transform.position.x || currentWaypoint.x <= transform.position.x && sprite.flipX;

		}
	}

	public void AttackTarget()
	{
		AttackUnit(targetedEnemy);
		targetedEnemy = null;
		queueAttack = false;
	}

	public void SetPath(Stack<Node> newPath)
	{
		ResetModifiers();
		unitUI.UpdateText(this);
		if (newPath == null)
		{
			Debug.LogError("Setting Null path!");
			return;
		}
		path = newPath;
		// Save final node
		DestinationNode = newPath.ToArray()[^1];
		SetNextWaypoint();
	}

	void SetNextWaypoint()
	{
		Node newNode = path.Pop();
		currentWaypoint = new(newNode.XPos + 0.5f, yOffset, newNode.YPos + 0.5f);
		IsMoving = true;
		if (GameManager.Instance.Clock.Consume(newNode.MovementCost))
		{
			path.Clear();
			DestinationNode = GameManager.Instance.World.GetNodeFromWorldPosition(transform.position);
			currentWaypoint = new(DestinationNode.XPos + 0.5f, yOffset, DestinationNode.YPos + 0.5f);
		}
	}

	public void SetTargetedEnemy(UnitEnemy enemy) => targetedEnemy = enemy;

	/// <summary>
	/// Grants the player  a given amount of xp
	/// </summary>
	/// <param name="xpToGain">The amount of xp for the player to gain</param>
	public void GainXP(int xpToGain)
	{
		Debug.Log("Gained XP!");
		int newXp = xp + xpToGain;
		if (newXp > XpToNextLevel)
		{
			Debug.Log("Levelled up!");
			xp = newXp - XpToNextLevel;
			LevelUp();
			xpDisplay.UpdateLevelText(Level);
		}
		else
		{
			xp = newXp;
		}
		xpDisplay.UpdateXP((float)xp / XpToNextLevel);
	}

	/// <summary>
	/// Levels the player up
	/// </summary>
	public void LevelUp()
	{
		IncreaseMaxHealth(1);
		HealDamage(1);
		Level++;
		healthDisplay.UpdateHearts(Health, MaxHealth);
		unitUI.UpdateText(this);
	}

	/// <summary>
	/// Permanently increases the unit's max health
	/// </summary>
	/// <param name="increaseAmount"></param>
	public void IncreaseMaxHealth(int increaseAmount) => MaxHealth += increaseAmount;

	/// <summary>
	/// Permanantly increases the unit's offence stat
	/// </summary>
	/// <param name="increaseAmount">The amount to increase the unit's offence</param>
	public void IncreaseOffence(int increaseAmount)
	{
		Offence += increaseAmount;
		statsDisplay.UpdateStats(this);
	}

	/// <summary>
	/// Permanantly increases the unit's defence stat
	/// </summary>
	/// <param name="increaseAmount">The amount to increase the unit's defence</param>
	public void IncreaseDefence(int increaseAmount)
	{
		Defence += increaseAmount;
		statsDisplay.UpdateStats(this);
	}

	public override void HealDamage(int healAmount)
	{
		base.HealDamage(healAmount);
		healthDisplay.UpdateHearts(Health, MaxHealth);
	}

	public override void TakeDamage(int damageAmount)
	{
		base.TakeDamage(damageAmount);
		healthDisplay.UpdateHearts(Health, MaxHealth);
	}

	public void TriggerGetItem(PlayerItemGet.Item i)
	{
		anim.SetTrigger("itemGet");
		itemGetSprite.Activate(i);
	}

	protected override void OnDeath()
	{
		// GameOver;
		// Bring up lose screen
		anim.SetTrigger("death");
		GameManager.Instance.GameOver();
	}
}
