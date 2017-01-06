using System.Collections;
using System.Collections.Generic;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class SettingsStateInterface : StateInterface
{
	private GameObject _settingsPanel;
	private SettingCreation _creator;
	private ButtonList _buttons;

	public override void Initialize()
	{
		_buttons = new ButtonList("SettingsContainer/SettingsPanelContainer/ButtonContainer");
		var backButton = _buttons.GetButton("BackButton");
		var applyButton = _buttons.GetButton("ApplyButton");
		_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/SettingsPanel");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		var language = _creator.Language(false);
		applyButton.onClick.AddListener(delegate { OnApplyClick(language); });
		backButton.onClick.AddListener(OnBackClick);
	}

	private void OnApplyClick(Dropdown language)
	{
		Localization.UpdateLanguage(language.value);
		_buttons.BestFit();
	}

	private void OnBackClick()
	{
		EnqueueCommand(new PreviousStateCommand());
	}

	public override void Enter()
	{
		_buttons.BestFit();

		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
	}

	public override void Exit()
	{
		GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
	}
}
