using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.Commands
{
	public class CommandQueue : CommandQueue<ICommand>
	{
	}

	public class CommandQueue<TCommand> : ICommandQueue<TCommand>
		where TCommand : ICommand
	{
        private readonly List<TCommand> _commands = new List<TCommand>();

        public bool HasCommands
        {
            get { return _commands.Any(); }
        }

        public void AddCommand(TCommand command)
        {
            _commands.Add(command);
        }

        public void AddCommands(IEnumerable<TCommand> commands)
        {
            _commands.AddRange(commands);
        }

        public TCommand TakeFirstCommand()
        {
            var command = _commands[0];
            _commands.RemoveAt(0);
            return command;
        }

        public TCommand[] TakeAllCommands()
        {
            var commands = _commands.ToArray();
            _commands.Clear();
            return commands;
        }
    }
}