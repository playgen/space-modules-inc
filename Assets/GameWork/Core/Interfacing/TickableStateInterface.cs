using System.Collections.Generic;
using GameWork.Core.Commands;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Interfacing.Interfaces;

namespace GameWork.Core.Interfacing
{
    public abstract class TickableStateInterface : StateInterface, ITickableStateInterface
    {
        public abstract void Tick(float deltaTime);
    }
}