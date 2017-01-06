using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class LoadingStateInterface : StateInterface
{
    public override void Initialize()
    {
        GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer/OfflineButton").GetComponent<Button>().onClick.AddListener(
            delegate
            {
                EnqueueCommand(new NextStateCommand());
            });

    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(true);
        // Load stuff
        SUGARManager.Account.DisplayPanel(success =>
        {
            if (success)
            {
                EnqueueCommand(new NextStateCommand());
            }
            else
            {
                Debug.LogError("Sign In FAAILL");
            }
        });
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
    }
}
