using GameWork.Core.States.Event;

public class FalseEventTransition : EventStateTransition
{
	private readonly string _toStateName;

	public FalseEventTransition(string toStateName)
	{
		_toStateName = toStateName;
	}

	public void ChangeState(bool value)                   
	{
		if (!value)
		{
			ExitState(_toStateName);
			EnterState(_toStateName);
		}
	}
}