using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using System.Linq;

using Object = UnityEngine.Object;

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
		Dropdown feedback;
		_creator.TryForPlatform<Dropdown>("FEEDBACK_MODE", true, out feedback, true, false);
		feedback.GetComponent<DropdownLocalization>().SetOptions(new List<string> { "FEEDBACK_" + ScenarioController.FeedbackMode.Minimal, "FEEDBACK_" + ScenarioController.FeedbackMode.EndGame, "FEEDBACK_" + ScenarioController.FeedbackMode.InGame });
		feedback.value = PlayerPrefs.GetInt("Feedback", (int)ScenarioController.FeedbackMode.Minimal);
		feedback.onValueChanged.AddListener(OnFeedbackChange);
		_feedbackMode = feedback.transform.parent.gameObject;
		Dropdown language;
		_creator.TryLanguageForPlatform(out language, true, false);
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

		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);

		if (_feedbackMode && SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("feedback"))
		{
			Object.Destroy(_feedbackMode);
		}
		_creator.RebuildLayout();
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
