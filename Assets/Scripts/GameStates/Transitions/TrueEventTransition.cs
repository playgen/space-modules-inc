using GameWork.Core.States.Event;

public class TrueEventTransition : EventStateTransition
{
	private readonly string _toStateName;

	public TrueEventTransition(string toStateName)
	{
		_toStateName = toStateName;
	}

	public void ChangeState(bool value)
	{
		if (value)
		{
			ExitState(_toStateName);
			EnterState(_toStateName);
		}
	}
}
