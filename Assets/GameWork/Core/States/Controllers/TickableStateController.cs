using GameWork.Core.Interfaces;
using GameWork.Core.States.Interfaces;

namespace GameWork.Core.States.Controllers
{
    public class TickableStateController : TickableStateController<ITickableState>
    {
        public TickableStateController(params ITickableState[] states) : base(states)
        {
        }
    }

    public class TickableStateController<TTickableState> : StateController<TTickableState>, ITickable
        where TTickableState : ITickableState
    {
        public TickableStateController(params TTickableState[] states) : base(states)
        {
        }

        public void Tick(float deltaTime)
        {
            if (States.ContainsKey(ActiveState))
            {
                States[ActiveState].Tick(deltaTime);
            }
        }
    }
}