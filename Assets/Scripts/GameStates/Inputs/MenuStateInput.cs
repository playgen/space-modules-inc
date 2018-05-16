﻿using System;
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
	private GameObject _gameLockedPanel;
	private bool _startTimeCheck = false;

	protected override void OnInitialize()
	{
		_playButton = GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/PlayButton").GetComponent<Button>();
		_playButton.onClick.AddListener(() => PlayClickedEvent?.Invoke());
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/SettingsButton").GetComponent<Button>().onClick.AddListener(() => SettingsClickedEvent?.Invoke());
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/LeaderboardButton").GetComponent<Button>().onClick.AddListener(() =>
		{
			SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
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
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel/AchievementButton").GetComponent<Button>().onClick.AddListener(() =>
		{
			SUGARManager.Evaluation.DisplayAchievementList();
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

		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.AddListener(Application.Quit);
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.AddListener(() => OnQuitAttempt(true));
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "MainMenuState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
		OnQuitAttempt(true);
		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer/MenuPanel").BestFit();

		GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);

		var gameLocked = CommandLineUtility.CustomArgs == null || CommandLineUtility.CustomArgs.Count == 0;
		if (!Application.isEditor && !gameLocked && !_startTimeCheck)
		{
			var dateTimeArg = string.Empty;
			DateTime launchTime;
			if (!CommandLineUtility.CustomArgs.TryGetValue("tstamp", out dateTimeArg) || !DateTime.TryParse(dateTimeArg, out launchTime) || DateTime.Now < launchTime || DateTime.Now.Subtract(launchTime).TotalHours > 1)
			{
				gameLocked = true;
			}
			_startTimeCheck = true;
		}
		_playButton.interactable = !gameLocked;
		_gameLockedPanel.SetActive(gameLocked);
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
			else
			{
				OnQuitAttempt(_quitPanel.activeSelf);
			}
		}
	}

	private void OnQuitAttempt(bool showMenu)
	{
		_menuPanel.SetActive(showMenu);
		_quitPanel.SetActive(!showMenu);
		_menuPanel.BestFit();
		_quitPanel.GetComponentsInChildren<Button>().ToList().Select(b => b.GetComponentInChildren<Text>()).BestFit();
	}
}
