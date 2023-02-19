using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void Quit() => Application.Quit();

	public void LoadNormal()
	{
		SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Normal;
		SceneManager.LoadScene(1);
	}
	
	public void LoadEndless()
	{
		SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Endless;
		SceneManager.LoadScene(1);
	}
}
