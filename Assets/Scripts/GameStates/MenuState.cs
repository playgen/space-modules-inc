﻿using GameWork.States;

public class MenuState : TickableSequenceState
{
    private MenuStateInterface _interface;
    public const string StateName = "MenuState";

    public MenuState(MenuStateInterface @interface)
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
        ChangeState(LoadingState.StateName);
    }

    public override void Tick(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
}