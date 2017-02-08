using System;

using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class LevelState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	public event Action CompletedEvent;

	public const string StateName = "LevelState";

	public LevelState(LevelStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnEnter()
	{
		Tracker.T.accessible.Accessed("LevelSelect", AccessibleTracker.Accessible.Screen);

		// Round based
		_scenarioController.NextLevel();
		
		//NextState();
	}

	public override string Name
	{
		get { return StateName; }
	}

	protected override void OnTick(float deltaTime)
	{
		if (CompletedEvent != null) CompletedEvent();
		return;


		ICommand command;
		if(CommandQueue.TryTakeFirstCommand(out command))
		{
			var refreshLevelDataCommand = command as RefreshLevelDataCommand;
			if (refreshLevelDataCommand != null)
			{
				refreshLevelDataCommand.Execute(_scenarioController);
			}

			var setLevelCommand = command as SetLevelCommand;
			if (setLevelCommand != null)
			{
				setLevelCommand.Execute(_scenarioController);
			}
		}
		
	}
}

