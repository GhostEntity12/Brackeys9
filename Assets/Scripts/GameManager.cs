using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public SaveData Save { get; private set; }
	public AudioSource AudioSource { get; private set; }

	new private void Awake()
	{
		base.Awake();
		Save = ReadWrite.Read();
		SceneLoadTypeData.Create();
		AudioSource = GetComponent<AudioSource>();
	}

	public void LerpAudio(float volume)
	{
		AudioSource.volume = volume;
	}
}
