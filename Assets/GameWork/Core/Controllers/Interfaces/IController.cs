using GameWork.Core.Interfaces;

namespace GameWork.Core.Controllers.Interfaces
{
	public interface IController : IInitializable, IActivatable, ITickable
	{
	}
}