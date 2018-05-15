using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class QuestionnaireState : InputTickState
{
	private readonly ScenarioController _scenarioController;

	public const string StateName = "QuestionnaireState";

	public override string Name => StateName;

	public QuestionnaireState(QuestionnaireStateInput input, ScenarioController scenarioController) : base(input)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var refreshPlayerDialogueCommand = command as RefreshPlayerDialogueCommand;
			refreshPlayerDialogueCommand?.Execute(_scenarioController);

			var setPlayerActionCommand = command as SetPlayerActionCommand;
			setPlayerActionCommand?.Execute(_scenarioController);

			var refreshCharacterResponseCommand = command as RefreshCharacterResponseCommand;
			refreshCharacterResponseCommand?.Execute(_scenarioController);
		}
	}
}
