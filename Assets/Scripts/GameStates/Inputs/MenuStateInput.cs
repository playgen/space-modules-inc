using System;
using System.Linq;

using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

using RAGE.Analytics;

using UnityEngine;

public class MenuStateInput : TickStateInput
{
	public event Action SettingsClickedEvent;
	public event Action PlayClickedEvent;

	private ButtonList _buttons;
	private Button _playButton;
	private GameObject _menuPanel;
	private GameObject _quitPanel;

	private bool _gameLocked;
	private GameObject _gameLockedPanel;

	protected override void OnInitialize()
	{
		_buttons = new ButtonList("MenuContainer/MenuPanelContainer/MenuPanel");
		_playButton = _buttons.GetButton("PlayButton");
		_playButton.onClick.AddListener(OnPlayClick);
		var settingsButton = _buttons.GetButton("SettingsButton");
		settingsButton.onClick.AddListener(OnSettingsClick);
		var leaderboardButton = _buttons.GetButton("LeaderboardButton");
		leaderboardButton.onClick.AddListener(delegate
		{
			SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
		});
		leaderboardButton.onClick.AddListener(delegate
		{
			Tracker.T.Accessible.Accessed("LeaderboardState", AccessibleTracker.Accessible.Screen);
		});
		var achievementButton = _buttons.GetButton("AchievementButton");
		achievementButton.onClick.AddListener(delegate
		{
			SUGARManager.Evaluation.DisplayAchievementList();
		});
		achievementButton.onClick.AddListener(delegate
		{
			Tracker.T.Accessible.Accessed("AchievementsState", AccessibleTracker.Accessible.Screen);
		});

		_menuPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel");
		_quitPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/QuitPanel");
		_gameLockedPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/GameLockedPanel");

		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.RemoveAllListeners();
		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.AddListener(Application.Quit);
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.RemoveAllListeners();
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.AddListener(OnQuitPanelNoClick);
	}

	private void OnSettingsClick()
	{
		if (SettingsClickedEvent != null) SettingsClickedEvent();
	}

	protected override void OnEnter()
	{
		Tracker.T.Accessible.Accessed("MainMenu", AccessibleTracker.Accessible.Screen);
		OnQuitPanelNoClick();
		_buttons.GameObjects.BestFit();

		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);

		_gameLocked = CommandLineUtility.CustomArgs == null || CommandLineUtility.CustomArgs.Count == 0;
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SUGARManager.Evaluation.IsActive)
			{
				SUGARManager.Evaluation.Hide();
			}
			else if (SUGARManager.Leaderboard.IsActive)
			{
				SUGARManager.Leaderboard.Hide();
			}
			else if (_quitPanel.activeSelf)
			{
				OnQuitPanelNoClick();
			}
			else
			{
				OnQuitAttempt();
			}
		}
		_playButton.interactable = !_gameLocked;
		_gameLockedPanel.SetActive(_gameLocked);
	}

	private void OnPlayClick()
	{
		if (PlayClickedEvent != null) PlayClickedEvent();
	}

	private void OnQuitAttempt()
	{
		_menuPanel.SetActive(false);
		_quitPanel.SetActive(true);
		_menuPanel.BestFit();
		_quitPanel.GetComponentsInChildren<Button>().ToList().Select(b => b.GetComponentInChildren<Text>()).BestFit();
	}

	private void OnQuitPanelNoClick()
	{
		_menuPanel.SetActive(true);
		_quitPanel.SetActive(false);
		_menuPanel.BestFit();
		_quitPanel.GetComponentsInChildren<Button>().ToList().Select(b => b.GetComponentInChildren<Text>()).BestFit();
	}
}
