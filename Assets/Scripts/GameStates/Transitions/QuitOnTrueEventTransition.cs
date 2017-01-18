using GameWork.Core.States.Event;
using UnityEngine;

public class QuitOnTrueEventTransition: EventStateTransition
{
	public void Quit(bool value)
	{
		if (value)
		{
			ExitState(null);
			Application.Quit();
		}
	}
}