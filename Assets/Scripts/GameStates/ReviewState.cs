using Assets.Scripts.Inputs;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class ReviewState : InputTickState
{
	private readonly ScenarioController _scenarioController;
	public const string StateName = "ReviewState";
	
	public ReviewState(ReviewStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
	}
	
	public override string Name
	{
		get { return StateName; }
	}
	
	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var getReviewDataCommand = command as GetReviewDataCommand;
			if (getReviewDataCommand != null)
			{
				getReviewDataCommand.Execute(_scenarioController);
			}
		}
	}
}
