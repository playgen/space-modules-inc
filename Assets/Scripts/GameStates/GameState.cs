﻿using Assets.Scripts.Inputs;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class GameState : InputTickState
{
    private readonly ScenarioController _scenarioController;
    private readonly ModulesController _modulesController;
	private readonly GameStateInput _input;

	public const string StateName = "GameState";

    public GameState(GameStateInput input, ScenarioController scenarioController, ModulesController modulesController) : base(input)
    {
	    _input = input;
        _scenarioController = scenarioController;
        _modulesController = modulesController;
    }
	
    public override string Name
    {
        get { return StateName; }
    }

    //public override void NextState()
    //{
    //    ChangeState(ReviewState.StateName);
    //}

    //public override void PreviousState()
    //{
    //    ChangeState(LevelState.StateName);
    //}

    protected override void OnTick(float deltaTime)
    {
	    ICommand command;
        if (CommandQueue.TryTakeFirstCommand(out command))
        {
	        var updateDialogueFontSizeCommand = command as UpdateDialogueFontSizeCommand;
	        if (updateDialogueFontSizeCommand != null)
	        {
		        updateDialogueFontSizeCommand.Execute(_input);
	        }

            var refreshPlayerDialogueCommand = command as RefreshPlayerDialogueCommand;
            if (refreshPlayerDialogueCommand != null)
            {
                refreshPlayerDialogueCommand.Execute(_scenarioController);
            }

            var setPlayerActionCommand = command as SetPlayerActionCommand;
            if (setPlayerActionCommand != null)
            {
                setPlayerActionCommand.Execute(_scenarioController);
            }

            var refreshCharacterResponseCommand = command as RefreshCharacterResponseCommand;
            if (refreshCharacterResponseCommand != null)
            {
                refreshCharacterResponseCommand.Execute(_scenarioController);
            }

            var toggleModulesCommand = command as ToggleModulesCommand;
            if (toggleModulesCommand != null)
            {
                toggleModulesCommand.Execute(_modulesController);
            }
        }
    }
}
