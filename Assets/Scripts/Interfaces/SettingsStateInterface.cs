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

    public override void Initialize()
    {
        _settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
        _creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
        _creator.Wipe();
        var language = _creator.Language(false);
        var buttonLayout = _creator.HorizontalLayout("Buttons");
        var back = _creator.Custom<Button>("Cancel", SettingObjectType.Button, true);
        _creator.AddToLayout(buttonLayout, back);
        back.onClick.AddListener(OnBackClick);
        var apply = _creator.Custom<Button>("Apply", SettingObjectType.Button, true);
        apply.onClick.AddListener(delegate { OnApplyClick(language); });
        _creator.AddToLayout(buttonLayout, apply);

    }

    private void OnApplyClick(Dropdown language)
    {
        Localization.UpdateLanguage(language.value);
    }

    private void OnBackClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
    }
}
