using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Commands.States.Interfaces;

namespace GameWork.Core.Commands.States
{
    public class ChangeStateCommand : ICommand<IChangeStateAction>
    {
        private readonly string _toStateName;

        public ChangeStateCommand(string toStateName)
        {
            _toStateName = toStateName;
        }

        public void Execute(IChangeStateAction implementor)
        {
            implementor.ChangeState(_toStateName);
        }
    }
}
