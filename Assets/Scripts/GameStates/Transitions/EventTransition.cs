using GameWork.Core.States.Event;

public class EventTransition : EventStateTransition
{
	private readonly string _toStateName;

	public EventTransition(string toStateName)
	{
		_toStateName = toStateName;
	}

	public void ChangeState()                   
	{
		ExitState(_toStateName);
		EnterState(_toStateName);
	}
}