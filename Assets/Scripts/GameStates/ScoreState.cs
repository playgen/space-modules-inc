using System;
using System.Collections.Generic;

using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

using PlayGen.SUGAR.Unity;

using TrackerAssetPackage;

using UnityEngine;

public class ScoreState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	public const string StateName = "ScoreState";
	public event Action<bool> NextEvent;
	public event Action InGameQuestionnaire;

	public ScoreState(ScoreStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
		input.NextButtonClicked += OnNextButtonClicked;
	}

	private void OnNextButtonClicked()
	{
		Debug.Log(_scenarioController.CurrentLevel + " / " + _scenarioController.LevelMax);
		var isGameOver = _scenarioController.CurrentLevel >= _scenarioController.LevelMax;

		if (isGameOver)
		{
			if (CommandLineUtility.CustomArgs.ContainsKey("lockafterq"))
			{
				var locked = bool.Parse(CommandLineUtility.CustomArgs["lockafterq"]);
				if (locked)
				{
					CommandLineUtility.CustomArgs = null;
				}
			}


			if (_scenarioController.UseInGameQuestionnaire)
			{
				InGameQuestionnaire?.Invoke();
			}
			else
			{
				if (_scenarioController.PostQuestions)
				{
					// The following string contains the key for the google form is used for the cognitive load questionnaire
					var formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

					// Google form ID
					var googleFormsURL = "https://docs.google.com/forms/d/e/"
											+ formsKey
											+ "/viewform?entry.1596836094="
											+ SUGARManager.CurrentUser.Name;

					TrackerEventSender.SendEvent(new TraceEvent("Questionnaire", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
					{

					}));

					// Open the default browser and show the form

					// TODO hand open url and quit in next state?
					Application.OpenURL(googleFormsURL);
					if (Application.platform == RuntimePlatform.WindowsPlayer)
					{
						Application.Quit();
					}
				}
			}
		}
		NextEvent?.Invoke(false);

		//if (NextEvent != null) NextEvent(isGameOver);
	}

	public override string Name => StateName;

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var getScoreDataCommand = command as GetScoreDataCommand;
			getScoreDataCommand?.Execute(_scenarioController);
		}
	}
}
