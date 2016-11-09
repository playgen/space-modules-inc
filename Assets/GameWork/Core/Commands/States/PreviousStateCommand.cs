using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Commands.States.Interfaces;

namespace GameWork.Core.Commands.States
{
    public class PreviousStateCommand : ICommand<IPreviousStateAction>
    {
        public void Execute(IPreviousStateAction implementor)
        {
            implementor.PreviousState();
        }
    }
}
