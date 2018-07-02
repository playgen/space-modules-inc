using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;

public class MenuStateInput : TickStateInput
{
	private readonly string _panelRoute = "MenuContainer/MenuPanelContainer";
	private readonly ScenarioController _scenarioController;

	private const string PilotTitle = "PILOT_MODE_TITLE";
	private const string PilotDescription = "PILOT_MODE_DESCRIPTION";
	private const string LockedTitle = "GAME_LOCK_TITLE";
	private const string LockedDescription = "GAME_LOCK_TEXT";
	private const string ExpiredTitle = "PILOT_MODE_EXPIRED_TITLE";
	private const string ExpiredDescription = "PILOT_MODE_EXPIRED_DESCRIPTION";

	public event Action SettingsClickedEvent;
	public event Action PlayClickedEvent;

	private GameObject _panel;
	private GameObject _background;
	private Button _playButton;
	private GameObject _menuPanel;
	private GameObject _quitPanel;
	private GameObject _messagePanel;
	private TimeSpan _startTimeGap = TimeSpan.MinValue;

	public MenuStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel/SettingsButton").GetComponent<Button>().onClick.AddListener(() => SettingsClickedEvent?.Invoke());
		GameObjectUtilities.FindGameObject(_panelRoute +  "/MenuPanel/LeaderboardButton").GetComponent<Button>().onClick.AddListener(() =>
		{
			SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
			SendTrackerEvent("ViewLeaderboard");
		});
		GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel/AchievementButton").GetComponent<Button>().onClick.AddListener(() =>
		{
			SUGARManager.Evaluation.DisplayAchievementList();
			SendTrackerEvent("ViewAchievements");
		});
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage");
		_playButton = GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel/PlayButton").GetComponent<Button>();
		_menuPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel");
		_quitPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/QuitPanel");
		_messagePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/MessagePanel");

		_messagePanel.SetActive(false);
		_playButton.onClick.AddListener(() => PlayClickedEvent?.Invoke());
		_quitPanel.transform.FindButton("YesButton").onClick.AddListener(Application.Quit);
		_quitPanel.transform.FindButton("NoButton").onClick.AddListener(() => OnQuitAttempt(true));
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "MainMenuState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});
		OnQuitAttempt(true);
		_menuPanel.BestFit();

		_panel.SetActive(true);
		_background.SetActive(true);

		if (_startTimeGap == TimeSpan.MinValue && SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("wipeprogress"))
		{
			PlayerPrefs.DeleteKey("CurrentLevel" + _scenarioController.RoundNumber);
			_scenarioController.CurrentLevel = 0;
		}
		var isPilot = SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs != null && CommandLineUtility.CustomArgs.Count != 0;
		var gameLocked = isPilot && _scenarioController.LevelMax > 0 && _scenarioController.CurrentLevel >= _scenarioController.LevelMax;

		var lockedTitleText = Localization.Get(LockedTitle, true);
		var lockedDescriptionText = Localization.Get(LockedDescription);
		
		// Game locking control for when in pilot
		if (isPilot)
		{
			if (_startTimeGap == TimeSpan.MinValue)
			{
				if (SUGARManager.CurrentUser != null && (CommandLineUtility.CustomArgs.ContainsKey("forcelaunch") || !CommandLineUtility.CustomArgs.ContainsKey("feedback")))
				{
					_startTimeGap = DateTimeOffset.Now.Subtract(DateTimeOffset.Now.AddSeconds(-10));
				}
				else
				{
					string dateTimeArg;
					DateTimeOffset launchTime;
					if (SUGARManager.CurrentUser == null || !CommandLineUtility.CustomArgs.TryGetValue("tstamp", out dateTimeArg) || !DateTimeOffset.TryParse(dateTimeArg, out launchTime))
					{
						gameLocked = true;
						_startTimeGap = TimeSpan.MaxValue;
					}
					else
					{
						_startTimeGap = DateTimeOffset.Now.Subtract(launchTime);
					}
				}
			}
			if (_startTimeGap.TotalSeconds < 0 || _startTimeGap.TotalHours >= 1)
			{
				gameLocked = true;
				lockedTitleText = Localization.Get(ExpiredTitle, true);
				lockedDescriptionText = Localization.Get(ExpiredDescription);
				Debug.LogWarning("Game Locked: Time Expired");
			}
		}
		// for iOS build, we cannot simply lock the game, see: Guideline 4.2.3 https://developer.apple.com/app-store/review/guidelines/#minimum-functionality
		_playButton.interactable = !gameLocked;

		_messagePanel.SetActive(isPilot);

		_messagePanel.transform.FindText("TitleText").text = gameLocked ? lockedTitleText : Localization.Get(PilotTitle);
		_messagePanel.transform.FindText("DescriptionText").text = gameLocked ? lockedDescriptionText : Localization.Get(PilotDescription);
	}

	protected override void OnExit()
	{
		_panel.SetActive(false);
		_background.SetActive(false);
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
		_quitPanel.GetComponentsInChildren<Button>().ToList().BestFit();
	}

	private void SendTrackerEvent(string key)
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.AssetActivity, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.AssetId, "SUGAR" },
			{ TrackerEvaluationKey.Action, key }
		});
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.Gamification, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.Event, key }
		});
	}
}
