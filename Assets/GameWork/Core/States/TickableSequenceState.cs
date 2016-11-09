using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States
{
    public abstract class TickableSequenceState : SequenceState, ITickableState
    {
        public abstract void Tick(float deltaTime);
    }
}
