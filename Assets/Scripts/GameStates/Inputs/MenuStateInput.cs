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

	public event Action SettingsClickedEvent;
	public event Action PlayClickedEvent;
	private Button _playButton;
	private GameObject _menuPanel;
	private GameObject _quitPanel;
	private GameObject _pilotModePanel;
	private GameObject _gameLockedPanel;
	private TimeSpan _startTimeGap = TimeSpan.MinValue;
    private readonly ScenarioController _scenarioController;

	private const string LockedTitle = "GAME_LOCK_TITLE";
	private const string LockedDescription = "GAME_LOCK_TEXT";

	private const string ExpiredTitle = "PILOT_MODE_EXPIRED_TITLE";
	private const string ExpiredDescription = "PILOT_MODE_EXPIRED_DESCRIPTION";

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
		_playButton = GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel/PlayButton").GetComponent<Button>();
		_menuPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel");
		_quitPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/QuitPanel");
		_pilotModePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/PilotModePanel");
		_gameLockedPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameLockedPanel");

		_pilotModePanel.SetActive(false);
		_playButton.onClick.AddListener(() => PlayClickedEvent?.Invoke());
		_quitPanel.transform.Find("YesButton").GetComponent<Button>().onClick.AddListener(Application.Quit);
		_quitPanel.transform.Find("NoButton").GetComponent<Button>().onClick.AddListener(() => OnQuitAttempt(true));
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
		GameObjectUtilities.FindGameObject(_panelRoute + "/MenuPanel").BestFit();

		GameObjectUtilities.FindGameObject(_panelRoute + "").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);

		if (_startTimeGap == TimeSpan.MinValue && SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("wipeprogress"))
		{
			PlayerPrefs.DeleteKey("CurrentLevel" + _scenarioController.RoundNumber);
			_scenarioController.CurrentLevel = 0;
		}
		var isPilot = SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs != null &&
		              CommandLineUtility.CustomArgs.Count != 0;
		var gameLocked = _scenarioController.CurrentLevel >= _scenarioController.LevelMax && isPilot;

		var lockedTitleText = Localization.Get(LockedTitle, true);
		var lockedDescriptionText = Localization.Get(LockedDescription);
		
		// Game locking control for when in pilot
		if (isPilot)
		{
			if (_startTimeGap == TimeSpan.MinValue)
			{
				if (SUGARManager.CurrentUser != null && (CommandLineUtility.CustomArgs.ContainsKey("forcelaunch") ||
				                                         !CommandLineUtility.CustomArgs.ContainsKey("feedback")))
				{
					_startTimeGap = DateTimeOffset.Now.Subtract(DateTimeOffset.Now.AddSeconds(-10));
				}
				else
				{
					string dateTimeArg;
					DateTimeOffset launchTime;
					if (SUGARManager.CurrentUser == null || !CommandLineUtility.CustomArgs.TryGetValue("tstamp", out dateTimeArg) ||
					    !DateTimeOffset.TryParse(dateTimeArg, out launchTime))
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

		_pilotModePanel.SetActive(isPilot && !gameLocked);
		_gameLockedPanel.SetActive(gameLocked);

		_gameLockedPanel.transform.FindText("TitleText").text = lockedTitleText;
		_gameLockedPanel.transform.FindText("DescriptionText").text = lockedDescriptionText;


	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject(_panelRoute + "").SetActive(false);
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
