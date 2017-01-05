using GameWork.Core.Math.Types;

namespace GameWork.Core.Components.Interfaces
{
    public interface ITransform : IComponent
    {
        Vector3 Position { get; set; }
    }
}
