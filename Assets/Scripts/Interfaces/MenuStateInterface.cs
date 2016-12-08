using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using PlayGen.SUGAR.Common.Shared;
using SUGAR.Unity;

public class MenuStateInterface : StateInterface
{
    public override void Initialize()
    {
        var buttons = new ButtonList("MenuContainer/MenuPanelContainer/MenuPanel");
        var playButton = buttons.GetButton("PlayButton");
        playButton.onClick.AddListener(OnPlayClick);
        var leaderboardButton = buttons.GetButton("LeaderboardButton");
        leaderboardButton.onClick.AddListener(delegate
        {
            SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
        });
    }

    public override void Enter()
    {
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
