using GameWork.Core.Commands.Accounts.Interfaces;
using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands.Accounts
{
    public class LogoutCommand : ICommand<ILogoutAction>
    {
        public void Execute(ILogoutAction implementor)
        {
            implementor.Logout();
        }
    }
}
