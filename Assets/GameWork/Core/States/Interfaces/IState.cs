using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Interfaces;
using System;

namespace GameWork.Core.States.Interfaces
{
	public interface IState : IInitializable, IEnterable
	{
		string Name { get; }

		bool IsActive { get; }

		event Action<string> ChangeStateEvent;

		event Action BackStateEvent;

		void ChangeState(string toStateName);

		void BackState();
	}
}