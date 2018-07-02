using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStateInput : TickStateInput
{
	private readonly string _panelRoute = "ScoreContainer/ScorePanelContainer";
	private readonly ScenarioController _scenarioController;

	public event Action NextEvent;
	public event Action InGameQuestionnaire;

	private GameObject _panel;
	private GameObject _background; 
	private ScorePanelBehaviour _scorePanel;

	public ScoreStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage");
		_scorePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/ScorePanel").GetComponent<ScorePanelBehaviour>();
		GameObjectUtilities.FindGameObject(_panelRoute + "/ScorePanel/NextButton").GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "ScoreState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});

		CommandQueue.AddCommand(new GetScoreDataCommand());
		_scenarioController.GetScoreDataSuccessEvent += _scorePanel.SetScorePanel;
		_panel.SetActive(true);
		_background.SetActive(true);
	}

	protected override void OnExit()
	{
		_scenarioController.GetScoreDataSuccessEvent -= _scorePanel.SetScorePanel;
		_panel.SetActive(false);
		_background.SetActive(false);
	}

	private void OnNextButtonClicked()
	{
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