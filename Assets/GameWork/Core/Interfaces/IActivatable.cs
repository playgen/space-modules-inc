namespace GameWork.Core.Interfaces
{
    public interface IActivatable
    {
        bool IsActive { get; }

        void Activate();

        void Deactivate();
    }
}
