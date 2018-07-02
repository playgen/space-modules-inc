using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Settings;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.Extensions;

public class SettingsStateInput : TickStateInput
{
	private readonly ScenarioController _scenarioController;
	private readonly string _panelRoute = "SettingsContainer/SettingsPanelContainer";

	public event Action BackClickedEvent;

	private GameObject _panel;
	private GameObject _background;
	private SettingCreation _creator;
	private GameObject _feedbackMode;

	public SettingsStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage");
		_creator = GameObjectUtilities.FindGameObject(_panelRoute + "/SettingsPanel").GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		Dropdown feedback;
		_creator.TryForPlatform("FEEDBACK_MODE", true, out feedback, true, false);
		feedback.GetComponent<DropdownLocalization>().SetOptions(new List<string> { "FEEDBACK_" + ScenarioController.FeedbackMode.Minimal, "FEEDBACK_" + ScenarioController.FeedbackMode.EndGame, "FEEDBACK_" + ScenarioController.FeedbackMode.InGame });
		feedback.value = PlayerPrefs.GetInt("Feedback", (int)ScenarioController.FeedbackMode.Minimal);
		feedback.onValueChanged.AddListener(OnFeedbackChange);
		_feedbackMode = feedback.Parent();
		Dropdown language;
		_creator.TryLanguageForPlatform(out language, true, false);
		language.onValueChanged.AddListener(OnLanguageChange);
		_creator.RebuildLayout();
		GameObjectUtilities.FindGameObject(_panelRoute + "/SettingsPanel/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);
	}

	private void OnLanguageChange(int value)
	{
		Localization.UpdateLanguage(Localization.Languages[value]);
		_creator.RebuildLayout();
	}

	private void OnFeedbackChange(int value)
	{
		_scenarioController.FeedbackLevel = (ScenarioController.FeedbackMode)value;
		PlayerPrefs.SetInt("Feedback", (int)_scenarioController.FeedbackLevel);
		_creator.RebuildLayout();
	}

	private void OnBackClick()
	{
		BackClickedEvent?.Invoke();
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "SettingsState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});

		_panel.SetActive(true);
		_background.SetActive(true);
		_feedbackMode.SetActive(!_feedbackMode || SUGARManager.CurrentUser == null || !CommandLineUtility.CustomArgs.ContainsKey("feedback"));
		_creator.RebuildLayout();
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
			OnBackClick();
		}
	}
}
