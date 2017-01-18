using Assets.Scripts.Inputs;
using GameWork.Core.States.Tick.Input;

public class LoadingState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	private readonly LoadingStateInput _input;

	public const string StateName = "LoadingState";

	public LoadingState(LoadingStateInput input, ScenarioController scenarioController) : base(input)
	{
		_input = input;
		_scenarioController = scenarioController;
	}

	public override string Name
	{
		get { return StateName; }
	}
	

	protected override void OnEnter()
	{
		_input.LoggedInEvent += _scenarioController.Initialize;
		
	}

	protected override void OnExit()
	{
		_input.LoggedInEvent -= _scenarioController.Initialize;

	}

	//public override void NextState()
	//{
	//    ChangeState(MenuState.StateName);
	//}

	//public override void PreviousState()
	//{
	//    throw new System.NotImplementedException();
	//}
}
