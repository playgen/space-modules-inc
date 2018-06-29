﻿using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Settings;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.SUGAR.Unity;

using Object = UnityEngine.Object;

public class SettingsStateInput : TickStateInput
{
	private readonly string _panelRoute = "SettingsContainer/SettingsPanelContainer";

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
		_settingsPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/SettingsPanel");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		Dropdown feedback;
		_creator.TryForPlatform("FEEDBACK_MODE", true, out feedback, true, false);
		feedback.GetComponent<DropdownLocalization>().SetOptions(new List<string> { "FEEDBACK_" + ScenarioController.FeedbackMode.EndGame, "FEEDBACK_" + ScenarioController.FeedbackMode.InReview, "FEEDBACK_" + ScenarioController.FeedbackMode.InGame });
		feedback.value = PlayerPrefs.GetInt("Feedback", (int)ScenarioController.FeedbackMode.EndGame);
		feedback.onValueChanged.AddListener(OnFeedbackChange);
		_feedbackMode = feedback.transform.parent.gameObject;
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

		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);

		if (_feedbackMode && SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("feedback"))
		{
			Object.Destroy(_feedbackMode);
		}
		_creator.RebuildLayout();
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
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
