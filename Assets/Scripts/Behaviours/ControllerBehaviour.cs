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

		FixScreenRatio();

		var scenarioController = new ScenarioController();

		_stateController = new TickableStateController<TickableSequenceState>(
			new LoadingState(scenarioController, new LoadingStateInterface()),
			new MenuState(new MenuStateInterface()),
			new LevelState(scenarioController, new LevelStateInterface()),
			new CallState(new CallStateInterface()),
			new GameState(scenarioController, new GameStateInterface())
			);
		_stateController.Initialize();
	}

	private void FixScreenRatio()
	{
		if (Camera.main.aspect > 1)
		{
			var portrait = 1/(Camera.main.aspect * Camera.main.aspect);
			var x = (1 - portrait)/2;
			var y = 0;
			var W = portrait;
			var H = 1;
			Camera.main.rect = new Rect(new Vector2(x, y), new Vector2(W,H));
		}
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
