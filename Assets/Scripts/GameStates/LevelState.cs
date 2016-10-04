using GameWork.States;

public class LevelState : TickableSequenceState
{
    private LevelStateInterface _interface;

    public const string StateName = "LevelState";

    public LevelState(LevelStateInterface @interface)
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

