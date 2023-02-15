using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : Unit
{
	const float LevelCurveStrength = 1.403f;
	const float TargetLevel = 10;

	[SerializeField]
	float yOffset = 0.02f;
	[SerializeField]
	float speed = 1f;
	[SerializeField]
	float allowedError = 0.01f;

	Stack<Node> path = new();
	Vector3 currentWaypoint;
	public Node DestinationNode { get; private set; }
	public bool IsMoving { get; private set; }

	UnitEnemy targettedEnemy = null;

	int xp;
	int xpToNextLevel
	{
		get
		{
			if (level <= TargetLevel)
			{
				return Mathf.FloorToInt(Mathf.Pow(LevelCurveStrength, level - 1) + 9);
			}
			else
			{
				return Mathf.FloorToInt((Mathf.Pow(LevelCurveStrength, TargetLevel - 1) + 9) + (level - TargetLevel) * 10);
			}
		}
	}


	// Start is called before the first frame update
	void Start()
	{
		AssignStats();
		DestinationNode = GameManager.Instance.World.GetNodeFromWorldPosition(transform.position);
		currentWaypoint = new(DestinationNode.XPos + 0.5f, yOffset, DestinationNode.YPos + 0.5f);
		transform.position = transform.position + Vector3.up * yOffset;
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(transform.position, currentWaypoint) < allowedError)
		{
			if (path.Count > 0)
			{
				SetNextWaypoint();
			}
			else
			{
				IsMoving = false;
				if (targettedEnemy)
				{
					AttackUnit(targettedEnemy);
				}
				return;
			}
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
		}
	}

	public void SetPath(Stack<Node> newPath)
	{
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
	}

	public void SetTargetedEnemy(UnitEnemy enemy) => targettedEnemy = enemy;

	/// <summary>
	/// Grants the player  a given amount of xp
	/// </summary>
	/// <param name="xpToGain">The amount of xp for the player to gain</param>
	public void GainXP(int xpToGain)
	{
		int newXp = xp + xpToGain;
		if (newXp > xpToNextLevel)
		{
			xp = newXp - xpToNextLevel;
			LevelUp();
		}
	}

	/// <summary>
	/// Levels the player up
	/// </summary>
	public void LevelUp()
	{
		IncreaseMaxHealth(1);
		HealDamage(1);
		level++;
	}

	/// <summary>
	/// Permanently increases the unit's max health
	/// </summary>
	/// <param name="increaseAmount"></param>
	public void IncreaseMaxHealth(int increaseAmount) => MaxHealth = increaseAmount;

	/// <summary>
	/// Permanantly increases the unit's offence stat
	/// </summary>
	/// <param name="increaseAmount">The amount to increase the unit's offence</param>
	public void IncreaseOffence(int increaseAmount) => Offence += increaseAmount;

	/// <summary>
	/// Permanantly increases the unit's defence stat
	/// </summary>
	/// <param name="increaseAmount">The amount to increase the unit's defence</param>
	public void IncreaseDefence(int increaseAmount) => Defence += increaseAmount;

	protected override void OnDeath()
	{
		// GameOver;
		// Bring up lose screen
		throw new System.NotImplementedException();
	}
}
