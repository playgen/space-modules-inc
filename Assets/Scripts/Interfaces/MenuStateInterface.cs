using GameWork.Commands.States;
using GameWork.Interfacing;

public class MenuStateInterface : StateInterface
{
    public override void Initialize()
    {
        var buttons = new ButtonList("MenuContainer/MenuPanelContainer/MenuPanel");
        var playButton = buttons.GetButton("PlayButton");
        playButton.onClick.AddListener(OnPlayClick);
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(false);
    }

    private void OnPlayClick()
    {
        EnqueueCommand(new NextStateCommand());
    }
}
