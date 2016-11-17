using GameWork.Core.States;

public class ScoreState : TickableSequenceState
{
    private readonly ScoreStateInterface _interface;
    private readonly ScenarioController _scenarioController;
    public const string StateName = "ScoreState";

    public ScoreState(ScenarioController scenarioController, ScoreStateInterface @interface)
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
        _scenarioController.GetScoreDataSuccessEvent += _interface.UpdateScore;
        _interface.Enter();
    }

    public override void Exit()
    {
        _scenarioController.GetScoreDataSuccessEvent -= _interface.UpdateScore;
        _interface.Exit();
    }

    public override string Name
    {
        get { return StateName; }
    }

    public override void NextState()
    {
        ChangeState(LevelState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(ReviewState.StateName);
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            var getScoreDataCommand = command as GetScoreDataCommand;
            if (getScoreDataCommand != null)
            {
                getScoreDataCommand.Execute(_scenarioController);
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
