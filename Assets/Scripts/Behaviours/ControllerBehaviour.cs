using GameWork.Core.Audio;
using GameWork.Core.States.Tick;

using UnityEngine;

public class ControllerBehaviour : MonoBehaviour
{
	private TickStateController _stateController;
	private AudioController _audioController;

	private void Awake()
	{
		FixScreenRatio();

		_audioController = Application.platform == RuntimePlatform.WindowsPlayer ? new AudioController(new StreamingAssetsAudioChannelFactory(), 1) : new AudioController(new ResourceAudioChannelFactory(), 1);
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

	private void Start()
	{
		_stateController.EnterState(LoadingState.StateName);
	}

	private void Update ()
	{
		_audioController?.Tick(Time.deltaTime);
		_stateController?.Tick(Time.deltaTime);
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ScreenCapture.CaptureScreenshot(System.DateTime.UtcNow.ToFileTimeUtc() + ".png");
		}
#endif
	}
}
