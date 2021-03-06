﻿using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class GameState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	private readonly ModulesController _modulesController;

	public const string StateName = "GameState";

	public GameState(GameStateInput input, ScenarioController scenarioController, ModulesController modulesController) : base(input)
	{
		_scenarioController = scenarioController;
		_modulesController = modulesController;
	}
	
	public override string Name => StateName;

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var refreshPlayerDialogueCommand = command as RefreshPlayerDialogueCommand;
			refreshPlayerDialogueCommand?.Execute(_scenarioController);

			var setPlayerActionCommand = command as SetPlayerActionCommand;
			setPlayerActionCommand?.Execute(_scenarioController);

			var refreshCharacterResponseCommand = command as RefreshCharacterResponseCommand;
			refreshCharacterResponseCommand?.Execute(_scenarioController);

			var toggleModulesCommand = command as ToggleModulesCommand;
			toggleModulesCommand?.Execute(_modulesController);

			var closeModulesCommand = command as CloseModulesCommand;
			closeModulesCommand?.Execute(_modulesController);
		}
	}
}
