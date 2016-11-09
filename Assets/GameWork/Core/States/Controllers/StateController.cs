using System.Collections.Generic;
using GameWork.Core.Commands.States.Interfaces;
using GameWork.Core.Interfaces;
using GameWork.Core.States.Interfaces;

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
		protected readonly Dictionary<string, TState> States  = new Dictionary<string, TState>();

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
			var newState = States[name];

			if(ActiveState != null)
			{ 
				var prevState = States[ActiveState];
				
				prevState.ChangeStateEvent -= ChangeState;
				prevState.Exit();
			}

			ActiveState = name;

			newState.ChangeStateEvent += ChangeState;
			newState.Enter();
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
    }
}