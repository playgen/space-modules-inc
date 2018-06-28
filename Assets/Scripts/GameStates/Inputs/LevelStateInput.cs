using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

public class LevelStateInput : TickStateInput
{
	private readonly string _panelRoute = "LevelContainer/LevelPanelContainer";

	public event Action BackClickedEvent;
	public event Action LoadLevelEvent;

	private GameObject _gridLayout;
	private GameObject _itemPrefab;

	private readonly ScenarioController _scenarioController;
	private int _columns;

	public LevelStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnEnter()
	{
		//Tracker.T.Accessible.Accessed("LevelState");
		_scenarioController.RefreshSuccessEvent += UpdateLevelList;
		_scenarioController.SetLevelSuccessEvent += LevelLoaded;
		_gridLayout = GameObjectUtilities.FindGameObject(_panelRoute + "/LevelPanel/Scroll View/Viewport/Content/GridLayout");
		ConfigureGridSize(3, 3);
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
		CommandQueue.AddCommand(new RefreshLevelDataCommand());
	}

	protected override void OnInitialize()
	{
		_itemPrefab = Resources.Load("Prefabs/LevelItem") as GameObject;
		GameObjectUtilities.FindGameObject(_panelRoute + "/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);
	}

	protected override void OnExit()
	{
		_scenarioController.RefreshSuccessEvent -= UpdateLevelList;
		_scenarioController.SetLevelSuccessEvent -= LevelLoaded;

		ClearList();
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
	}

	private void OnBackClick()
	{
		BackClickedEvent?.Invoke();
	}

	private void LoadLevel(int id)
	{
		CommandQueue.AddCommand(new SetLevelCommand(id));
		//LoadLevelEvent?.Invoke();
	}

	private void LevelLoaded()
	{
		LoadLevelEvent?.Invoke();
	}

	private void ClearList()
	{
		// Clear List
		foreach (var cell in _gridLayout.transform)
		{
			var cellGameObject = cell as Transform;
			if (cellGameObject != null) UnityEngine.Object.Destroy(cellGameObject.gameObject);
		}
	}

	public void UpdateLevelList(ScenarioController.LevelObject[] Levels)
	{
		for (var i = 0; i < Levels.Length; i++)
		{
			var levelItem = UnityEngine.Object.Instantiate(_itemPrefab);
			levelItem.GetComponent<LevelItemBehaviour>().SetupItem(Levels[i].Stars, Localization.GetAndFormat("LINE", true, Levels[i].Id));
			levelItem.transform.SetParent(_gridLayout.transform, false);
			var index = i;
			levelItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				LoadLevel(Levels[index].Id);
			});
		}
		var contentView = _gridLayout.transform.parent.GetComponent<RectTransform>();
		var originalHeight = contentView.rect.height;
		var cellHeight = _gridLayout.GetComponent<GridLayoutGroup>().cellSize.y;
		var rows = (Mathf.Ceil(Levels.Length / (float)_columns));
		var contentHieght = cellHeight * rows;
		contentView.sizeDelta = new Vector2(1f, contentHieght-originalHeight);
	}

	private void ConfigureGridSize(int rows, int cols)
	{
		_columns = cols;
		var gridRect = _gridLayout.GetComponent<RectTransform>();
		var buttonHeight = gridRect.rect.height / rows;
		var buttonWidth = gridRect.rect.width / cols;
		var gridLayoutGroup = _gridLayout.GetComponent<GridLayoutGroup>();
		gridLayoutGroup.cellSize = new Vector2(buttonWidth, buttonHeight);
	}
}
