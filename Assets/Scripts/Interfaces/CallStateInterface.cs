using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;

public class CallStateInterface : StateInterface
{
    public override void Initialize()
    {
        var answerButton = GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/AnswerButton").GetComponent<Button>();
        answerButton.onClick.AddListener(OnAnswerClick);
    }

    private void OnAnswerClick()
    {
        EnqueueCommand(new NextStateCommand());
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
        GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StartAnimation();
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
        GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StopAnimation();

    }
}
