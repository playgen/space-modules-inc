using System.Collections.Generic;

namespace GameWork.Core.Commands.Interfaces
{
	public interface ICommandResolver : ICommandResolver<ICommand>
	{
	}

	public interface ICommandResolver<TCommand>
		where TCommand : ICommand
	{
		void ProcessCommand(TCommand command);

		void ProcessCommands(IEnumerable<TCommand> commmands);
	}
}
