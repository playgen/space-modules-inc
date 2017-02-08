using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;

public class SettingsStateInput : TickStateInput
{
	public event Action BackClickedEvent;

	private GameObject _settingsPanel;
	private SettingCreation _creator;
	private ButtonList _buttons;

	protected override void OnInitialize()
	{
		_buttons = new ButtonList("SettingsContainer/SettingsPanelContainer/SettingsPanel/ButtonContainer");
		var backButton = _buttons.GetButton("BackButton");
		var applyButton = _buttons.GetButton("ApplyButton");
		_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		var language = _creator.Language(false);
		applyButton.onClick.AddListener(delegate { OnApplyClick(language); });
		backButton.onClick.AddListener(OnBackClick);
		_creator.RebuildLayout();
	}

	private void OnApplyClick(Dropdown language)
	{
		Localization.UpdateLanguage(language.value);
		_buttons.GameObjects.BestFit();
	}

	private void OnBackClick()
	{
		if (BackClickedEvent != null) BackClickedEvent();
	}

	protected override void OnEnter()
	{
		_buttons.GameObjects.BestFit();

		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
	}
}
