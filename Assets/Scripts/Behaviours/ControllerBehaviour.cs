using GameWork.Core.Audio;
using GameWork.Core.States.Tick;

using UnityEngine;
using PlayGen.SUGAR.Unity;
using UnityEngine.SocialPlatforms.GameCenter;

public class ControllerBehaviour : MonoBehaviour
{

	private TickStateController _stateController;
	private AudioController _audioController;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);

		Social.Active = new GameCenterPlatform();

		FixScreenRatio();

		_audioController = new AudioController(new StreamingAssetsAudioChannelFactory());
		var scenarioController = new ScenarioController(_audioController);
		var modulesController = new ModulesController();

		var gameStateControllerFactory = new GameStateControllerFactory(scenarioController, modulesController);
		_stateController = gameStateControllerFactory.Create();
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
		if (_audioController != null)
		{
			_audioController.Tick(Time.deltaTime);
		}
		if (_stateController != null)
		{
			_stateController.Tick(Time.deltaTime);
		}

	}
}
