using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States
{
    public abstract class TickableState : State, ITickableState
    {
        public abstract void Tick(float deltaTime);
    }
}