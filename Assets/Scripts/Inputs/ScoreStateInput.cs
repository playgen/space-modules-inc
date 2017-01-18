﻿using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
	public class ScoreStateInput : TickStateInput
	{
		public event Action NextButtonClicked;
	
		private ScorePanelBehaviour _scorePanelScript;
		private readonly ScenarioController _scenarioController;

		public ScoreStateInput(ScenarioController scenarioController)
		{
			_scenarioController = scenarioController;
		}

		protected override void OnInitialize()
		{
			_scorePanelScript = GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel").GetComponent<ScorePanelBehaviour>();

			var nextButton =
				GameObjectUtilities.FindGameObject("ScoreContainer/ScorePanelContainer/ScorePanel/NextButton");
			nextButton.GetComponent<Button>().onClick.AddListener(() => NextButtonClicked());
		}
	
		protected override void OnEnter()
		{
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
			_scorePanelScript.SetScorePanel(obj.Stars, obj.Score, obj.ScoreFeedbackToken, obj.MoodImage, obj.EmotionCommentToken, obj.Bonus);
		}
	}
}


