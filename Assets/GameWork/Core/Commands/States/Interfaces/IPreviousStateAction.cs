using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.States.Interfaces
{
    public interface IPreviousStateAction : ICommandAction
    {
        void PreviousState();
    }
}
