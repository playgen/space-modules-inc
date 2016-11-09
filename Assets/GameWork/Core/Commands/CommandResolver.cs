using System.Collections.Generic;
using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands
{
	public abstract class CommandResolver : CommandResolver<ICommand>
	{
	}

	public abstract class CommandResolver<TCommand> : ICommandResolver<TCommand>
		where TCommand : ICommand
	{
		public abstract void ProcessCommand(TCommand command);

		public void ProcessCommands(IEnumerable<TCommand> commands)
		{
			if(commands == null) return;

			foreach (var command in commands)
			{
				ProcessCommand(command);
			}
		}

		public void ProcessCommandQueue(ICommandQueue<TCommand> commandQueue)
		{
			if (commandQueue == null) return;

			ProcessCommands(commandQueue.TakeAllCommands());
		}
	}
}
