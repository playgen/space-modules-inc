using GameWork.Core.States.Tick.Input;

public class LoadingState : InputTickState
{
	public const string StateName = "LoadingState";

	public LoadingState(LoadingStateInput input) : base(input)
	{
	}

	public override string Name => StateName;
}
