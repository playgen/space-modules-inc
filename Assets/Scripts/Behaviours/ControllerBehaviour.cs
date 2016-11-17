﻿using UnityEngine;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;

public class ControllerBehaviour : MonoBehaviour
{

	private TickableStateController<TickableSequenceState> _stateController;

	void Awake ()
	{
		DontDestroyOnLoad(transform.gameObject);

		FixScreenRatio();

		var scenarioController = new ScenarioController();
		var modulesController = new ModulesController();

		_stateController = new TickableStateController<TickableSequenceState>(
			new LoadingState(scenarioController, new LoadingStateInterface()),
			new MenuState(new MenuStateInterface()),
			new LevelState(scenarioController, new LevelStateInterface()),
			new CallState(new CallStateInterface()),
			new GameState(scenarioController, modulesController, new GameStateInterface()),
            new ReviewState(scenarioController, new ReviewStateInterface()),
            new ScoreState(scenarioController, new ScoreStateInterface())
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
			var w = portrait;
			var h = 1;
			Camera.main.rect = new Rect(new Vector2(x, y), new Vector2(w,h));
		}
	}

	void Start()
	{
		_stateController.ChangeState(LoadingState.StateName);
	}

	void Update ()
	{
		_stateController.Tick(Time.deltaTime);
	}
}
