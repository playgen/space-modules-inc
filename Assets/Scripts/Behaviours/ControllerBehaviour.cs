using Assets.Scripts.Inputs;
using GameWork.Core.Audio;
using GameWork.Core.States.Tick;
using GameWork.Unity.Engine.States;
using UnityEngine;
using PlayGen.SUGAR.Unity;
using UnityEngine.SocialPlatforms.GameCenter;

public class ControllerBehaviour : MonoBehaviour
{

	private TickStateController _stateController;
	private AudioController _audioController;

	void Awake ()
	{
		DontDestroyOnLoad(transform.gameObject);

		Social.Active = new GameCenterPlatform();

		FixScreenRatio();

		_audioController = new AudioController(new StreamingAssetsAudioChannelFactory());
		var scenarioController = new ScenarioController(_audioController);
		var modulesController = new ModulesController();

		var loadingState = CreateLoadingState(scenarioController);
		var menuState = CreateMenuState();
		var settingsState = CreateSettingsState();
		var levelState = CreateLevelState(scenarioController);
		var callState = CreateCallState();
		var gameState = CreateGameState(scenarioController, modulesController);
		var reviewState = CreateReviewState(scenarioController);
		var ScoreState = CreateScoreState(scenarioController);


		_stateController = new TickStateController(
			loadingState,
			menuState,
			settingsState,
			levelState,
			callState,
			gameState,
			reviewState,
			ScoreState
			);
		_stateController.Initialize();
	}

	private LoadingState CreateLoadingState(ScenarioController scenarioController)
	{
		var input = new LoadingStateInput();
		var state = new LoadingState(input, scenarioController);

		return state;
	}

	private MenuState CreateMenuState()
	{
		var input = new MenuStateInput();
		var state = new MenuState(input);

		return state;
	}

	private SettingsState CreateSettingsState()
	{
		var input = new SettingsStateInput();
		var state = new SettingsState(input);

		return state;
	}

	private LevelState CreateLevelState(ScenarioController scenarioController)
	{
		var input = new LevelStateInput();
		var state = new LevelState(input, scenarioController);

		return state;
	}

	private CallState CreateCallState()
	{
		var input = new CallStateInput();
		var state = new CallState(input);

		return state;
	}

	private GameState CreateGameState(ScenarioController scenarioController, ModulesController modulesController)
	{
		var input = new GameStateInput(scenarioController);
		var state = new GameState(input, scenarioController, modulesController);

		return state;
	}

	private ReviewState CreateReviewState(ScenarioController scenarioController)
	{
		var input = new ReviewStateInput(scenarioController);
		var state = new ReviewState(input, scenarioController);

		return state;
	}
	
	private ScoreState CreateScoreState(ScenarioController scenarioController)
	{
		var input = new ScoreStateInput(scenarioController);
		var state = new ScoreState(input, scenarioController);
		return state;
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
		_stateController.EnterState(LoadingState.StateName);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			SUGARManager.Achievement.DisplayList();
		}
		_audioController.Tick(Time.deltaTime);
		_stateController.Tick(Time.deltaTime);
		
	}
}
