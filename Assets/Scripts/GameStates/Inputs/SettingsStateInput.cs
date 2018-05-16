using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;
using PlayGen.Unity.Utilities.Localization;

public class SettingsStateInput : TickStateInput
{
	public event Action BackClickedEvent;

	private GameObject _settingsPanel;
	private SettingCreation _creator;
	private GameObject _feedbackMode;

	private readonly ScenarioController _scenarioController;

	public SettingsStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		var feedback = _creator.Custom<Dropdown>("FEEDBACK_MODE", true, true, false);
		feedback.GetComponent<DropdownLocalization>().SetOptions(new List<string> { "FEEDBACK_" + ScenarioController.FeedbackMode.Minimal, "FEEDBACK_" + ScenarioController.FeedbackMode.EndGame, "FEEDBACK_" + ScenarioController.FeedbackMode.InGame });
		feedback.value = PlayerPrefs.GetInt("Feedback", (int)ScenarioController.FeedbackMode.Minimal);
		feedback.onValueChanged.AddListener(OnFeedbackChange);
		_feedbackMode = feedback.transform.parent.gameObject;
		var language = _creator.Language(true, false);
		language.onValueChanged.AddListener(OnLanguageChange);
		_creator.RebuildLayout();
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);
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
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "SettingsState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});

		_feedbackMode.SetActive(_scenarioController.LevelMax == 0);
		_creator.RebuildLayout();

		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackClick();
		}
	}
}
