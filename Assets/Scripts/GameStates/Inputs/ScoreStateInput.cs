﻿using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStateInput : TickStateInput
{
	public event Action NextEvent;
	public event Action InGameQuestionnaire;

	private ScorePanelBehaviour _scorePanel;
	private readonly ScenarioController _scenarioController;
	private GameObject _nextButton;

	public ScoreStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_scorePanel = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel").GetComponent<ScorePanelBehaviour>();
		_nextButton = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel/NextButton");
		_nextButton.GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);
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
		_scenarioController.GetScoreDataSuccessEvent += UpdateScore;
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
		_scorePanel.SetScorePanel(obj);
	}

	private void OnNextButtonClicked()
	{
		if (_scenarioController.CurrentLevel >= _scenarioController.LevelMax)
		{
			if (CommandLineUtility.CustomArgs.ContainsKey("lockafterq"))
			{
				var locked = bool.Parse(CommandLineUtility.CustomArgs["lockafterq"]);
				if (locked)
				{
					CommandLineUtility.CustomArgs = null;
				}
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