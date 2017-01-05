using System;
using System.Collections.Generic;
using GameWork.Core.Commands.States.Interfaces;
using GameWork.Core.Interfaces;
using GameWork.Core.States.Interfaces;
using GameWork.Core.Commands.Interfaces;

namespace GameWork.Core.States.Controllers
{
    public class StateController : StateController<IState>
    {
        public StateController(params IState[] states) : base(states)
        {
        }
    }

    public class StateController<TState> : IInitializable, IChangeStateAction
		where TState : IState
	{
		public event Action<string> ChangeParentStateEvent;

		protected readonly Dictionary<string, TState> States  = new Dictionary<string, TState>();

		private string _backStateName;

		public string ActiveState { get; protected set; }

		public StateController(params TState[] states)
		{
			foreach (var state in states)
			{
				States.Add(state.Name, state);
			}
		}

		public void ChangeState(string name)
		{
			if (!States.ContainsKey(name))
			{
				ChangeParentStateEvent(name);
			}
			else
			{
				var newState = States[name];

				if (ActiveState != null)
				{
					_backStateName = ActiveState;
					var prevState = States[ActiveState];
					ExitState(prevState);
				}

				ActiveState = name;

				newState.ChangeStateEvent += ChangeState;
				newState.BackStateEvent += PreviousState;
				newState.Enter();
			}
		}

		public void ExitActiveState()
        {
            if (States.ContainsKey(ActiveState))
            {
                States[ActiveState].Exit();
            }
        }
        
		public void Initialize()
		{
			foreach (var state in States.Values)
			{
				state.Initialize();
			}
		}

		public void Terminate()
		{
            ExitActiveState();

			foreach (var state in States.Values)
			{
				state.Terminate();
			}
		}

		private void PreviousState()
		{
			ChangeState(_backStateName);
		}

		private void ExitState(TState state)
		{
			state.ChangeStateEvent -= ChangeState;
			state.BackStateEvent -= PreviousState;
			state.Exit();
		}
    }
}