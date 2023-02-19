using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	CanvasGroup fade;

	[SerializeField]
	Button endlessModeButton;
	[SerializeField]
	Image endlessModeText;

	private void Start()
	{
		bool unlocked = GameManager.Instance.Save.endlessModeUnlocked;
		endlessModeButton.interactable = unlocked;
		endlessModeText.color = unlocked ? Color.black : Color.grey;
	}

	public void Quit() => Application.Quit();

	public void LoadNormal()
	{
		SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Normal;
		LeanTween.alphaCanvas(fade, 1, 0.5f).setOnComplete(()=> SceneManager.LoadScene(1));
	}
	
	public void LoadEndless()
	{
		SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Endless;
		LeanTween.alphaCanvas(fade, 1, 0.5f).setOnComplete(() => SceneManager.LoadScene(1));
	}
}
