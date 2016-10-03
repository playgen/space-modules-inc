using GameWork.States;

public class GameState : TickableSequenceState
{
    private readonly GameStateInterface _interface;

    public GameState(GameStateInterface @interface)
    {
        _interface = @interface;
    }

    public const string StateName = "GameState";

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
