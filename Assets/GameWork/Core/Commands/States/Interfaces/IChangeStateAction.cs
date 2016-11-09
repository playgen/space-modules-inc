using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.States.Interfaces
{
    public interface IChangeStateAction : ICommandAction
    {
        void ChangeState(string toStateName);
    }
}
