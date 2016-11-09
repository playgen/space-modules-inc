using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.Accounts.Interfaces
{
    public interface ILoginAction : ICommandAction
    {
        void Login(string username, string password);
    }
}
