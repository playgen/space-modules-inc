using System.Collections;
using System.Collections.Generic;
using GameWork.Core.States;
using UnityEngine;

public class SettingsState : TickableSequenceState
{
    private readonly SettingsStateInterface _interface;
    public const string StateName = "SettingsState";

    public SettingsState(SettingsStateInterface @interface)
    {
        _interface = @interface;
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
        _interface.Enter();
    }

    public override void Exit()
    {
        _interface.Exit();
    }

    public override string Name
    {
        get { return StateName; }
    }

    public override void NextState()
    {
        throw new System.NotImplementedException();
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);

    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            //var getScoreDataCommand = command as GetScoreDataCommand;
            //if (getScoreDataCommand != null)
            //{
            //    getScoreDataCommand.Execute(_scenarioController);
            //}

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
