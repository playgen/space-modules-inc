using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using SUGAR.Unity;
using UnityEngine.UI;

public class ScoreStateInterface : StateInterface
{
    private ScorePanelBehaviour _scorePanelScript;

    public override void Initialize()
    {
        _scorePanelScript = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel").GetComponent<ScorePanelBehaviour>();

        var nextButton =
                  GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel/NextButton");
        nextButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            EnqueueCommand(new NextStateCommand());
        });
    }

    public override void Enter()
    {
        EnqueueCommand(new GetScoreDataCommand());
        GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
    }

    public void UpdateScore(ScenarioController.ScoreObject obj)
    {
        
        _scorePanelScript.SetScorePanel(obj.Stars, obj.Score, obj.ScoreFeedbackToken, obj.MoodImage, obj.EmotionCommentToken, obj.Bonus);
    }
}


