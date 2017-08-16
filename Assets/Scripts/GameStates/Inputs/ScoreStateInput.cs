using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

public class ScoreStateInput : TickStateInput
{
	public event Action NextButtonClicked;

	private ScorePanelBehaviour _scorePanelScript;
	private readonly ScenarioController _scenarioController;
	private GameObject _nextButton;

	public ScoreStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_scorePanelScript = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel").GetComponent<ScorePanelBehaviour>();

		_nextButton = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel/NextButton");

		_nextButton.GetComponent<Button>().onClick.AddListener(() => {
			if (NextButtonClicked != null) NextButtonClicked();
		});
	}

	protected override void OnEnter()
	{
		Tracker.T.accessible.Accessed("ScoreState", AccessibleTracker.Accessible.Screen);

		CommandQueue.AddCommand(new GetScoreDataCommand());
		_scenarioController.GetScoreDataSuccessEvent += UpdateScore;
		if (_scenarioController.CurrentLevel == _scenarioController.LevelMax)
		{
			_nextButton.GetComponentInChildren<TextLocalization>().Key = "EXIT";
			_nextButton.GetComponentInChildren<TextLocalization>().Set();
		}
		GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
	}

	protected override void OnExit()
	{
		_scenarioController.GetScoreDataSuccessEvent -= UpdateScore;
		GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
	}

	public void UpdateScore(ScenarioController.ScoreObject obj)
	{
		_scorePanelScript.SetScorePanel(obj.Stars, obj.Score, obj.ScoreFeedbackToken, obj.MoodImage, obj.EmotionCommentToken, obj.Bonus);
	}
}


