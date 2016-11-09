using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Interfaces;

namespace GameWork.Core.Interfacing.Interfaces
{
    public interface IStateInterface : IInitializable, IEnterable, ICommandQueue
    {
    }
}
