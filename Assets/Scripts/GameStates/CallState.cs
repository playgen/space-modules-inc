using GameWork.Core.States;

public class CallState : TickableSequenceState
{
    private CallStateInterface _interface;

    public const string StateName = "CallState";

    public CallState(CallStateInterface @interface)
    {
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
    }

    public override void Exit()
    {
        _interface.Exit();
    }
    public override void NextState()
    {
        ChangeState(GameState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(LevelState.StateName);
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
