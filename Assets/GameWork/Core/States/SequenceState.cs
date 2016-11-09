using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States
{
    public abstract class SequenceState : State, ISequenceState
    {
        public abstract void NextState();

        public abstract void PreviousState();
    }
}
