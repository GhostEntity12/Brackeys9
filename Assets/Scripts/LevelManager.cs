using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField]
	World worldPrefab;
	[SerializeField]
	UnitPlayer playerPrefab;
	[SerializeField]
	DoomsdayClock clockPrefab;
	[SerializeField]
	CrayonPath crayonPathPrefab;

	[SerializeField]
	HealthDisplay healthDisplayPrefab;
	[SerializeField]
	StatsDisplay statsDisplayPrefab;
	[SerializeField]
	XPDisplay xpDisplayPrefab;

	[Header("Victory")]
	[SerializeField]
	CanvasGroup bgFade;
	[SerializeField]
	RectTransform endScreenContainer;
	[SerializeField]
	EndScreen victory;
	[SerializeField]
	EndScreen defeat;

	public World World { get; private set; }
	UnitPlayer player;
	DoomsdayClock clock;
	CrayonPath crayonPath;


	UnitEnemy hoveredEnemy = null;
	Stack<Node> path;
	bool gameEnded;
	public bool InEndlessMode { get; private set; }

	public bool AllowInput { get; private set; }


	private void Awake()
	{
		World = Instantiate(worldPrefab);
		clock = Instantiate(clockPrefab);
		crayonPath = Instantiate(crayonPathPrefab);
		player = Instantiate(playerPrefab);

		World.SetPlayer(player);
		World.SetLevelManager(this);
		player.SetDisplays(Instantiate(healthDisplayPrefab), Instantiate(statsDisplayPrefab), Instantiate(xpDisplayPrefab));
		LeanTween.delayedCall(0.5f, () => AllowInput = true);


		if (SceneLoadTypeData.GetInstance() == null)
		{
			SceneLoadTypeData.Create();
			SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Endless;
		}
		InEndlessMode = SceneLoadTypeData.GetInstance().loadType == SceneLoadTypeData.LoadType.Endless;
	}

	private void Start()
	{
		World.Generate();
	}

	// Update is called once per frame
	void Update()
	{
		if (!AllowInput || gameEnded || player.IsMoving) return;

		// On tile hover
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitTile, 20, 1 << 6) &&
			hitTile.transform.TryGetComponent(out Tile tile))
		{
			// On tile click
			if (Input.GetMouseButtonDown(0))
			{
				player.SetPath(new(path.Reverse()));
				player.SetTargetedEnemy(hoveredEnemy);
			}

			// Check if hit enemy
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitEnemy, 20, 1 << 7) &&
				hitEnemy.transform.TryGetComponent(out UnitEnemy enemy) &&
				!enemy.IsDead &&
				World.GetNodeFromWorldPosition(enemy.transform.position) == tile.Node)
			{
				// Find best path
				int lowestCost = 10000;
				Stack<Node> lowestCostPath = null;
				for (int i = 0; i < 4; i++)
				{
					// Cast to ITuple to iterate through
					if ((World.GetNodeFromWorldPosition(enemy.transform.position).Neighbours as ITuple)[i] is not Node enemyNeighbourNode) continue;

					Stack<Node> comparisonPath = Pathfinding.Pathfind(player.DestinationNode, enemyNeighbourNode);
					if (comparisonPath == null) continue;

					int pathCost = Pathfinding.GetCost(comparisonPath);
					if (pathCost < lowestCost)
					{
						lowestCost = pathCost;
						lowestCostPath = comparisonPath;
					}
				}

				path = lowestCostPath ?? path;
				PreviewPath(path);

				hoveredEnemy = enemy;
			}
			else
			{
				player.SetTargetedEnemy(null);

				path = Pathfinding.Pathfind(player.DestinationNode, tile.Node) ?? path;
				PreviewPath(path);
			}
		}
	}

	void PreviewPath(Stack<Node> path)
	{
		crayonPath.Display(path);
		clock.ShowPreview(Pathfinding.GetCost(path));
	}

	public void Victory()
	{
		gameEnded = true;
		GameManager.Instance.Save.endlessModeUnlocked = true;

		victory.CanvasGroup.alpha = 1;
		victory.CanvasGroup.interactable = true;
		victory.CanvasGroup.blocksRaycasts = true;
		defeat.CanvasGroup.interactable = false;
		defeat.CanvasGroup.blocksRaycasts = false;

		LeanTween.alphaCanvas(bgFade, 1, 0.5f);
		LeanTween.moveY(endScreenContainer, 0, 0.5f).setEaseOutBack();

		victory.ScoreText.text = $"You beat Sketchbook Quest in\r\n {World.Generations} worlds!";
		if (GameManager.Instance.Save.bestScoreNormalGeneration == 0)
		{
			GameManager.Instance.Save.bestScoreNormalGeneration = World.Generations + 1;
		}
		if (World.Generations >= GameManager.Instance.Save.bestScoreNormalGeneration)
		{
			victory.HighscoreText.text = $"Previous best:\n  {GameManager.Instance.Save.bestScoreNormalGeneration} worlds";
		}
		else
		{
			GameManager.Instance.Save.bestScoreNormalGeneration = World.Generations;
			victory.HighscoreText.text = "New Personal Best!";
		}

		_ = ReadWrite.Write(GameManager.Instance.Save);

		LeanTween.scale(victory.Stamp, Vector3.one, 0.75f).setEaseInQuint().setDelay(0.6f);
		LeanTween.alpha(victory.Stamp, 1, 0.75f).setEaseInQuint().setDelay(0.6f);
	}

	public void GameOver()
	{
		gameEnded = true;

		defeat.CanvasGroup.alpha = 1;
		defeat.CanvasGroup.interactable = true;
		defeat.CanvasGroup.blocksRaycasts= true;
		victory.CanvasGroup.interactable = false;
		victory.CanvasGroup.blocksRaycasts= false;

		LeanTween.alphaCanvas(bgFade, 1, 0.5f);
		LeanTween.moveY(endScreenContainer, 0, 0.5f).setEaseOutBack();

		if (InEndlessMode)
		{
			defeat.ScoreText.text = $"You survived {World.Generations} worlds\r\n and made it to Level {player.Level}";
			if (player.Level <= GameManager.Instance.Save.bestScoreEndlessLevel)
			{
				defeat.HighscoreText.text = $"Previous best:\n  Level{GameManager.Instance.Save.bestScoreEndlessLevel}";
			}
			else
			{
				GameManager.Instance.Save.bestScoreEndlessLevel = World.Generations;
				defeat.HighscoreText.text = "New Personal Best!";
			}
		}
		else
		{
			defeat.ScoreText.text = $"You survived {World.Generations} worlds\r\n and made it to Level {player.Level}";
			defeat.HighscoreText.text = $"Previous best:\n  {GameManager.Instance.Save.bestScoreNormalGeneration} worlds";
		}

		LeanTween.scale(defeat.Stamp, Vector3.one, 0.75f).setEaseInQuint().setDelay(0.6f);
		LeanTween.alpha(defeat.Stamp, 1, 0.75f).setEaseInQuint().setDelay(0.6f);
	}

	public void ToggleAllowInput(bool allow) => AllowInput = allow;

	public void ReloadLevel() => SceneManager.LoadScene(1);
	public void LoadMenu() => SceneManager.LoadScene(0);
	public void LoadEndless()
	{
		SceneLoadTypeData.GetInstance().loadType = SceneLoadTypeData.LoadType.Endless;
		SceneManager.LoadScene(1);
	}
}

[Serializable]
public class EndScreen
{
	[field: SerializeField]
	public CanvasGroup CanvasGroup { get; private set; }
	[field: SerializeField]
	public TextMeshProUGUI ScoreText { get; private set; }
	[field: SerializeField]
	public TextMeshProUGUI HighscoreText { get; private set; }
	[field: SerializeField]
	public RectTransform Stamp { get; private set; }
}