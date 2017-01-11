using GameWork.Core.States;
using UnityEngine;

public class ScoreState : TickableSequenceState
{
	private readonly ScoreStateInterface _interface;
	private readonly ScenarioController _scenarioController;
	public const string StateName = "ScoreState";

	public ScoreState(ScenarioController scenarioController, ScoreStateInterface @interface)
	{
		_interface = @interface;
		_scenarioController = scenarioController;
	}

	public override void Initialize()
	{
		_interface.Initialize();
	}

	public override void Terminate()
	{
		_interface.Terminate();
	}

	public override void Enter()
	{
		_scenarioController.GetScoreDataSuccessEvent += _interface.UpdateScore;
		_interface.Enter();
	}

	public override void Exit()
	{
		_scenarioController.GetScoreDataSuccessEvent -= _interface.UpdateScore;
		_interface.Exit();
	}

	public override string Name
	{
		get { return StateName; }
	}

	public override void NextState()
	{
		//ChangeState(LevelState.StateName);
		if (_scenarioController.CurrentLevel != _scenarioController.LevelMax)
		{
			ChangeState(MenuState.StateName);
		}
		else
		{
			// The following string contains the key for the google form is used for the cognitive load questionnaire
			string formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

			// Google form ID
			string googleFormsURL = "https://docs.google.com/forms/d/e/"
				+ formsKey
				+ "/viewform?";

			// Open the default browser and show the form
			Application.OpenURL(googleFormsURL);
			Application.Quit();
		}
	}

	public override void PreviousState()
	{
		ChangeState(ReviewState.StateName);
	}

	public override void Tick(float deltaTime)
	{
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();

			var getScoreDataCommand = command as GetScoreDataCommand;
			if (getScoreDataCommand != null)
			{
				getScoreDataCommand.Execute(_scenarioController);
			}

			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}
}
