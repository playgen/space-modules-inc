using System;
using GameWork.Core.Commands.States.Interfaces;
using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States
{
	public abstract class State : IState, IChangeStateAction
	{
		public abstract string Name { get; }

		public bool IsActive { get; private set; }

		public event Action<string> ChangeStateEvent;

		public void ChangeState(string toStateName)
		{
			ChangeStateEvent(toStateName);
		}

		public virtual void Enter()
		{
			IsActive = true;
		}

		public virtual void Exit()
		{
			IsActive = false;
		}
		
		public virtual void Initialize()
		{

		}

		public virtual void Terminate()
		{

		}
	}
}