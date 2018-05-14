using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

public class SettingsStateInput : TickStateInput
{
	public event Action BackClickedEvent;

	private GameObject _settingsPanel;
	private SettingCreation _creator;

	protected override void OnInitialize()
	{
		_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		var language = _creator.Language(true, false);
		_creator.RebuildLayout();
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel/ButtonContainer/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel/ButtonContainer/ApplyButton").GetComponent<Button>().onClick.AddListener(() => OnApplyClick(language));
	}

	private void OnApplyClick(Dropdown language)
	{
		Localization.UpdateLanguage(Localization.Languages[language.value]);
		_creator.RebuildLayout();
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel/ButtonContainer").BestFit();
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

		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel/ButtonContainer").BestFit();
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
