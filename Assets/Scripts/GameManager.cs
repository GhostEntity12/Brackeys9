using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public UnitPlayer player;
	public Unit enemy;

	// Start is called before the first frame update
	void Start()
	{
		player.AttackUnit(enemy);
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void GenerateWorld()
	{

	}
}
