using GameWork.Core.Commands.States.Interfaces;
using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States.Controllers
{
	public class SequenceStateController : SequenceStateController<ISequenceState>
	{
		public SequenceStateController(params ISequenceState[] states) : base(states)
		{
		}
	}

	public class SequenceStateController<TSequenceState> : StateController<TSequenceState>, INextStateAction, IPreviousStateAction
		where TSequenceState : ISequenceState
	{
		public SequenceStateController(params TSequenceState[] states) : base(states)
		{
		}

		public void NextState()
		{
			States[ActiveState].NextState();
		}

		public void PreviousState()
		{
			States[ActiveState].PreviousState();
		}
	}
}