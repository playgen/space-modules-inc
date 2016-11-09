using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

public class LoadingStateInterface : StateInterface
{
    public override void Initialize()
    {
        
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(true);
        // Load stuff
        EnqueueCommand(new NextStateCommand());
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
    }
}
