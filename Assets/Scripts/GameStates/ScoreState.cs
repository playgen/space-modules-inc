using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class ScoreState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	public const string StateName = "ScoreState";

	public ScoreState(ScoreStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
	}

	public override string Name => StateName;

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var getScoreDataCommand = command as GetScoreDataCommand;
			getScoreDataCommand?.Execute(_scenarioController);
		}
	}
}
