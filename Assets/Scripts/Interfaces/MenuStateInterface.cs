using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MenuStateInterface : StateInterface
{
	private ButtonList _buttons;
	private Button _playButton;

	public override void Initialize()
    {
        _buttons = new ButtonList("MenuContainer/MenuPanelContainer/MenuPanel");
        _playButton = _buttons.GetButton("PlayButton");
        _playButton.onClick.AddListener(OnPlayClick);
        var settingsButton = _buttons.GetButton("SettingsButton");
        settingsButton.onClick.AddListener(OnSettingsClick);
        var leaderboardButton = _buttons.GetButton("LeaderboardButton");
        leaderboardButton.onClick.AddListener(delegate
        {
            SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
        });
    }

    private void OnSettingsClick()
    {
        EnqueueCommand(new ChangeStateCommand(SettingsState.StateName));
    }

    public override void Enter()
    {
		_buttons.BestFit("PlayButton");
        GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
    }

    private void OnPlayClick()
    {
        EnqueueCommand(new NextStateCommand());
        //Tracker.T.accessible.
    }
}
