using System.Collections.Generic;
using GameWork.Core.Commands;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.Interfacing.Interfaces;

namespace GameWork.Core.Interfacing
{
	public abstract class StateInterface : IStateInterface
    {
		private readonly CommandQueue _commandQueue = new CommandQueue();
		
		public bool HasCommands
		{
			get { return _commandQueue.HasCommands; }
		}

		public ICommand TakeFirstCommand()
		{
			return _commandQueue.TakeFirstCommand();
		}

		public ICommand[] TakeAllCommands()
		{
			return _commandQueue.TakeAllCommands();
		}
		
		public abstract void Enter();

		public abstract void Exit();

		public virtual void Initialize()
		{
		}

		public virtual void Terminate()
		{
		}

		protected void EnqueueCommand(ICommand command)
		{
			_commandQueue.AddCommand(command);
		}

		protected void EnqueueCommands(IEnumerable<ICommand> commands)
		{
			_commandQueue.AddCommands(commands);
		}
	}
}
