using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;

public class ScoreStateInput : TickStateInput
{
	private readonly string _panelRoute = "ScoreContainer/ScorePanelContainer";

	public event Action NextEvent;
	public event Action InGameQuestionnaire;

	private ScorePanelBehaviour _scorePanel;
	private readonly ScenarioController _scenarioController;

	public ScoreStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_scorePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/ScorePanel").GetComponent<ScorePanelBehaviour>();
		GameObjectUtilities.FindGameObject(_panelRoute + "/ScorePanel/NextButton").GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "ScoreState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});

		CommandQueue.AddCommand(new GetScoreDataCommand());
		_scenarioController.GetScoreDataSuccessEvent += _scorePanel.SetScorePanel;
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
	}

	protected override void OnExit()
	{
		_scenarioController.GetScoreDataSuccessEvent -= _scorePanel.SetScorePanel;
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
	}

	private void OnNextButtonClicked()
	{
		if (_scenarioController.CurrentLevel >= _scenarioController.LevelMax)
		{
			bool parseLockAfterQ;
			if (SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("lockafterq") && bool.TryParse(CommandLineUtility.CustomArgs["lockafterq"], out parseLockAfterQ) && parseLockAfterQ)
			{
				_scenarioController.CurrentLevel = _scenarioController.LevelMax;
			}
			else
			{
				_scenarioController.CurrentLevel = 0;
			}
		}
		if (_scenarioController.UseInGameQuestionnaire && _scenarioController.CurrentLevel % 5 == 0)
		{
			InGameQuestionnaire?.Invoke();
		}
		else
		{
			NextEvent?.Invoke();
		}
	}
}