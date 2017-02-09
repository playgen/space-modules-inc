using GameWork.Core.States.Event;

public class FinalLevelEventTransition : EventStateTransition
{
	private readonly string _toStateName;

	public FinalLevelEventTransition(string toStateName)
	{
		_toStateName = toStateName;
	}

	public void ChangeState()
	{
		ExitState(_toStateName);
		EnterState(_toStateName);
	}
}