using System;
using System.Collections.Generic;
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
	private Button _playButton;
	private GameObject _menuPanel;
	private GameObject _quitPanel;

	private bool _gameLocked;
	private GameObject _gameLockedPanel;

	protected override void OnInitialize()
	{
		_playButton = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/PlayButton").GetComponent<Button>();
		_playButton.onClick.AddListener(OnPlayClick);
		var settingsButton = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/SettingsButton").GetComponent<Button>(); ;
		settingsButton.onClick.AddListener(OnSettingsClick);
		var leaderboardButton = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/LeaderboardButton").GetComponent<Button>(); ;
		leaderboardButton.onClick.AddListener(delegate
		{
			SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
		});
		leaderboardButton.onClick.AddListener(delegate
		{
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.AssetActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Asset, "SUGAR" },
				{ TrackerEvaluationKeys.Done, "true" }
			});
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.Gamification, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ViewLeaderboard" }
			});
		});
		var achievementButton = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/AchievementButton").GetComponent<Button>(); ;
		achievementButton.onClick.AddListener(delegate
		{
			SUGARManager.Evaluation.DisplayAchievementList();
		});
		achievementButton.onClick.AddListener(delegate
		{
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.AssetActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Asset, "SUGAR" },
				{ TrackerEvaluationKeys.Done, "true" }
			});
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.Gamification, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ViewAchievements" }
			});
		});

		_menuPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel");
		_quitPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/QuitPanel");
		_gameLockedPanel = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/GameLockedPanel");
		_gameLockedPanel.SetActive(false);

		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.RemoveAllListeners();
		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.AddListener(Application.Quit);
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.RemoveAllListeners();
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.AddListener(OnQuitPanelNoClick);
	}

	private void OnSettingsClick()
	{
		SettingsClickedEvent?.Invoke();
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "MainMenuState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
		OnQuitPanelNoClick();
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel").BestFit();

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
		PlayClickedEvent?.Invoke();
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
