using GameWork.States;

public class LevelState : TickableSequenceState
{
    private LevelStateInterface _interface;
    private ScenarioController _scenarioController;

    public const string StateName = "LevelState";

    public LevelState(ScenarioController scenarioController, LevelStateInterface @interface)
    {
        _scenarioController = scenarioController;
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
        _scenarioController.RefreshSuccessEvent += _interface.UpdateLevelList;
        _interface.Enter();
    }

    public override void Exit()
    {
        _scenarioController.RefreshSuccessEvent -= _interface.UpdateLevelList;
        _interface.Exit();
    }

    public override void NextState()
    {
        ChangeState(CallState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);
    }

    public override string Name
    {
        get { return StateName; }
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

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
            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}

