using GameWork.Interfacing;

public class GameStateInterface : StateInterface
{
    public override void Initialize()
    {
        
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
    }
}
