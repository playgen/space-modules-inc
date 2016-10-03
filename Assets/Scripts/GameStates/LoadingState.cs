using GameWork.States;

public class LoadingState : TickableSequenceState
{
    private LoadingStateInterface _interface;

    public const string StateName = "LoadingState";

    public LoadingState(LoadingStateInterface @interface)
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
        ChangeState(MenuState.StateName);
    }

    public override void PreviousState()
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
}
