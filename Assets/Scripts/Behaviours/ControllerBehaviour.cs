using UnityEngine;
using System.Collections;
using GameWork.States;
using UnityEngine.Networking;

public class ControllerBehaviour : MonoBehaviour
{

	private TickableStateController<TickableSequenceState> _stateController;

	void Awake ()
	{
		DontDestroyOnLoad(transform.gameObject);

		_stateController = new TickableStateController<TickableSequenceState>(
			new LoadingState(new LoadingStateInterface()),
			new MenuState(new MenuStateInterface()),
			new LevelState(new LevelStateInterface()),
            new CallState(new CallStateInterface()),
            new GameState(new GameStateInterface())
			);
		_stateController.Initialize();
	}

	void Start()
	{
		_stateController.SetState(LoadingState.StateName);
	}

	void Update ()
	{
		_stateController.Tick(Time.deltaTime);
	}
}
