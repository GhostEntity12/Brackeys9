using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public SaveData Save { get; private set; }

	private void Start()
	{
		Save = ReadWrite.Read();
	}
}
