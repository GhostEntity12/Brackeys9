using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	readonly (int xSize, int ySize) WorldSize = (11, 11);

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
