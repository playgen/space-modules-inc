using System;
using System.Collections.Generic;

using GameWork.Core.States.Tick.Input;

using PlayGen.Unity.Utilities.Extensions;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

public class LevelStateInput : TickStateInput
{
	private readonly string _panelRoute = "LevelContainer/LevelPanelContainer";
	private readonly ScenarioController _scenarioController;

	public event Action BackClickedEvent;
	public event Action LoadLevelEvent;

	private GameObject _panel;
	private GameObject _background;
	private GridLayoutGroup _gridLayout;
	private GameObject _itemPrefab;

	private int _columns;

	public LevelStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage");
		_itemPrefab = Resources.Load<GameObject>("Prefabs/LevelItem");
		_gridLayout = GameObjectUtilities.FindGameObject(_panelRoute + "/LevelPanel/Scroll View/Viewport/Content/GridLayout").GetComponent<GridLayoutGroup>();
		GameObjectUtilities.FindGameObject(_panelRoute + "/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "LevelSelectState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});
		_scenarioController.RefreshSuccessEvent += UpdateLevelList;
		_scenarioController.SetLevelSuccessEvent += LevelLoaded;
		ConfigureGridSize(3);
		_panel.SetActive(true);
		_background.SetActive(true);
		CommandQueue.AddCommand(new RefreshLevelDataCommand());
	}

	protected override void OnExit()
	{
		_scenarioController.RefreshSuccessEvent -= UpdateLevelList;
		_scenarioController.SetLevelSuccessEvent -= LevelLoaded;

		ClearList();
		_panel.SetActive(false);
		_background.SetActive(false);
	}

	private void OnBackClick()
	{
		BackClickedEvent?.Invoke();
	}

	/// <summary>
	/// Called on set level command success to ensure that the level has been successfully loaded before continuing
	/// </summary>
	private void LevelLoaded()
	{
		LoadLevelEvent?.Invoke();
	}

	/// <summary>
	/// Clears the current levels in the grid layout
	/// </summary>
	private void ClearList()
	{
		// Clear List
		foreach (Transform cell in _gridLayout.transform)
		{
			UnityEngine.Object.Destroy(cell.gameObject);
		}
	}

	/// <summary>
	/// Iterate through the provided levels and populate the grid layout
	/// </summary>
	/// <param name="levels"></param>
	public void UpdateLevelList(ScenarioController.LevelObject[] levels)
	{
		for (var i = 0; i < levels.Length; i++)
		{
			var levelItem = UnityEngine.Object.Instantiate(_itemPrefab, _gridLayout.transform, false);
			levelItem.GetComponent<LevelItemBehaviour>().SetupItem(levels[i].Stars, Localization.GetAndFormat("LINE", true, levels[i].Id));
			var index = i;
			levelItem.GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new SetLevelCommand(levels[index].Id)));
		}
		var contentView = _gridLayout.Parent().RectTransform();
		var cellHeight = _gridLayout.cellSize.y;
		var rows = Mathf.Ceil(levels.Length / (float)_columns);
		var contentHieght = cellHeight * rows;
		contentView.sizeDelta = new Vector2(1f, contentHieght);
	}

	/// <summary>
	/// Set the number of columns for the grid, grid is embedded in a scroll view so rows is unlimeted.
	/// Elements will be forced to be square based on button width
	/// </summary>
	/// <param name="cols"></param>
	private void ConfigureGridSize(int cols)
	{
		_columns = cols;
		var gridRect = _gridLayout.RectTransform();
		var buttonWidth = gridRect.rect.width / cols;
		_gridLayout.cellSize = new Vector2(buttonWidth, buttonWidth);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackClick();
		}
	}
}
