using System;
using System.Linq;

using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

using UnityEngine;

public class MenuStateInput : TickStateInput
{
	public event Action SettingsClickedEvent;
	public event Action PlayClickedEvent;

	private ButtonList _buttons;
	private Button _playButton;
	private GameObject _menuPanel;
	private GameObject _quitPanel;

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
			Tracker.T.accessible.Accessed("LeaderboardState", AccessibleTracker.Accessible.Screen);
		});
		var achievementButton = _buttons.GetButton("AchievementButton");
		achievementButton.onClick.AddListener(delegate
		{
			SUGARManager.Achievement.DisplayList();
		});
		achievementButton.onClick.AddListener(delegate
		{
			Tracker.T.accessible.Accessed("AchievementsState", AccessibleTracker.Accessible.Screen);
		});
		_menuPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel");
		_quitPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/QuitPanel");
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
		Tracker.T.accessible.Accessed("MainMenu", AccessibleTracker.Accessible.Screen);
		OnQuitPanelNoClick();
		_buttons.GameObjects.BestFit();
		_playButton.GetComponentInChildren<Text>().fontSize *= 2;
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
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
			if (SUGARManager.Achievement.IsActive)
			{
				SUGARManager.Achievement.Hide();
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
