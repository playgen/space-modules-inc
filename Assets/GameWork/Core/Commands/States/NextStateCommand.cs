using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Commands.States.Interfaces;

namespace GameWork.Core.Commands.States
{
    public class NextStateCommand : ICommand<INextStateAction>
    {
        public void Execute(INextStateAction implementor)
        {
            implementor.NextState();
        }
    }
}
