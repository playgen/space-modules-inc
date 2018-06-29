using GameWork.Core.Audio;
using GameWork.Core.States.Tick;

using UnityEngine;

public class ControllerBehaviour : MonoBehaviour
{
	private TickStateController _stateController;
	private AudioController _audioController;

	private void Awake()
	{
		_audioController = new AudioController(new ResourceAudioChannelFactory(), 1);
		var scenarioController = new ScenarioController(_audioController);
		var modulesController = new ModulesController();

		var gameStateControllerFactory = new GameStateControllerFactory(scenarioController, modulesController);
		_stateController = gameStateControllerFactory.Create();
	}

	private void Start()
	{
		_stateController?.EnterState(LoadingState.StateName);
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
