using GameWork.Core.States;

public class ReviewState : TickableSequenceState
{
    private readonly ReviewStateInterface _interface;
    private ScenarioController _scenarioController;
    public const string StateName = "ReviewState";


    public ReviewState(ScenarioController scenarioController, ReviewStateInterface @interface)
    {
        _interface = @interface;
        _scenarioController = scenarioController;
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
        _scenarioController.GetChatHistorySuccessEvent += _interface.BuildChatHistory;
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
        ChangeState(GameState.StateName);
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            //var refreshPlayerDialogueCommand = command as RefreshPlayerDialogueCommand;
            //if (refreshPlayerDialogueCommand != null)
            //{
            //    refreshPlayerDialogueCommand.Execute(_scenarioController);
            //}

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
