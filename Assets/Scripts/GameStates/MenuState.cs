using GameWork.Core.States;

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
		Tracker.T.accessible.Accessed("MainMenu", AccessibleTracker.Accessible.Screen);
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
		ChangeState(LevelState.StateName);
	}

	public override void PreviousState()
	{
		ChangeState(LoadingState.StateName);
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
