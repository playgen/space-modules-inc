﻿using System;

using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

using RAGE.Analytics;

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
		Tracker.T.Accessible.Accessed("LevelSelect", AccessibleTracker.Accessible.Screen);

		// Round based
		_scenarioController.NextLevel();
		
		//NextState();
	}

	public override string Name => StateName;

	protected override void OnTick(float deltaTime)
	{
		CompletedEvent?.Invoke();
		return;


		ICommand command;
		if(CommandQueue.TryTakeFirstCommand(out command))
		{
			var refreshLevelDataCommand = command as RefreshLevelDataCommand;
			refreshLevelDataCommand?.Execute(_scenarioController);

			var setLevelCommand = command as SetLevelCommand;
			setLevelCommand?.Execute(_scenarioController);
		}
		
	}
}

