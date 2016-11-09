namespace GameWork.Core.Commands.Interfaces
{
    public interface ICommand
    {
    }

    public interface ICommand<TCommandAction> : ICommand
        where TCommandAction : ICommandAction
    {
        void Execute(TCommandAction implementor);
    }
}