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

        var scenarioController = new ScenarioController();

		_stateController = new TickableStateController<TickableSequenceState>(
			new LoadingState(scenarioController, new LoadingStateInterface()),
			new MenuState(new MenuStateInterface()),
			new LevelState(scenarioController, new LevelStateInterface()),
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
