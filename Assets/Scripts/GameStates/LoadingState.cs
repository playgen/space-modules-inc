using GameWork.States;

public class LoadingState : TickableSequenceState
{
    private LoadingStateInterface _interface;
    private ScenarioController _scenarioController;

    public const string StateName = "LoadingState";

    public LoadingState(ScenarioController scenarioController, LoadingStateInterface @interface)
    {
        _scenarioController = scenarioController;
        _interface = @interface;
    }

    public override string Name
    {
        get { return StateName; }
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
        _scenarioController.Initialize();
    }

    public override void Exit()
    {
        _interface.Exit();
    }

    public override void NextState()
    {
        ChangeState(MenuState.StateName);
    }

    public override void PreviousState()
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            //var quickGameCommand = command as QuickGameCommand;
            //if (quickGameCommand != null)
            //{
            //    quickGameCommand.Execute(_controller);
            //}

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
