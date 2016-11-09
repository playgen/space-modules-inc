using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.Accounts.Interfaces
{
    public interface ILogoutAction : ICommandAction
    {
        void Logout();
    }
}
