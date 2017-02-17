using System;

using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

using PlayGen.SUGAR.Unity;

using UnityEngine;

public class ScoreState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	public const string StateName = "ScoreState";
	public event Action<bool> NextEvent;

	public ScoreState(ScoreStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
		input.NextButtonClicked += OnNextButtonClicked;
	}

	private void OnNextButtonClicked()
	{
		var isGameOver = _scenarioController.CurrentLevel == _scenarioController.LevelMax;

		if (isGameOver && _scenarioController.PostQuestions)
		{
			// The following string contains the key for the google form is used for the cognitive load questionnaire
			string formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

			// Google form ID
			string googleFormsURL = "https://docs.google.com/forms/d/e/"
									+ formsKey
									+ "/viewform?entry.1596836094="
									+ SUGARManager.CurrentUser.Name;

			Tracker.T.accessible.Accessed("Questionnaire");


			// Open the default browser and show the form
			Application.OpenURL(googleFormsURL);
			Application.Quit();

		}

		Tracker.T.RequestFlush();

		if (NextEvent != null) NextEvent(isGameOver);
	}

	public override string Name
	{
		get { return StateName; }
	}
	
	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var getScoreDataCommand = command as GetScoreDataCommand;
			if (getScoreDataCommand != null)
			{
				getScoreDataCommand.Execute(_scenarioController);
			}
		}
	}
}
