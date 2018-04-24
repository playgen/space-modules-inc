using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class LevelState : InputTickState
{
	private readonly ScenarioController _scenarioController;

	public const string StateName = "LevelState";

	public LevelState(LevelStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
	}

	public override string Name => StateName;

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if(CommandQueue.TryTakeFirstCommand(out command))
		{
			var refreshLevelDataCommand = command as RefreshLevelDataCommand;
			refreshLevelDataCommand?.Execute(_scenarioController);

			var setLevelCommand = command as SetLevelCommand;
			setLevelCommand?.Execute(_scenarioController);
		}
		
	}
}

