using GameWork.Core.Commands.States.Interfaces;

namespace GameWork.Core.States.Interfaces
{
	public interface ISequenceState : IState, INextStateAction, IPreviousStateAction
	{
	}
}
