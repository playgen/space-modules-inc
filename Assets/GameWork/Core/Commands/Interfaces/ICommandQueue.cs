namespace GameWork.Core.Commands.Interfaces
{
	public interface ICommandQueue : ICommandQueue<ICommand>
	{
	}

	public interface ICommandQueue<TCommand>
		where TCommand : ICommand
	{
		bool HasCommands { get; }

		TCommand TakeFirstCommand();

		TCommand[] TakeAllCommands();
	}
}