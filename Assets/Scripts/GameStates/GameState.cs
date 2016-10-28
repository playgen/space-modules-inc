using System.Diagnostics;
using GameWork.States;

public class GameState : TickableSequenceState
{
    private readonly GameStateInterface _interface;
    private ScenarioController _controller;

    public const string StateName = "GameState";

    public GameState(ScenarioController controller, GameStateInterface @interface)
    {
        _interface = @interface;
        _controller = controller;
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
        _interface.ShowCharacter(_controller.CurrentCharacter);
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
