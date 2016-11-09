using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.Accounts.Interfaces
{
    public interface IRegisterAction : ICommandAction
    {
        void Register(string username, string password);
    }
}
