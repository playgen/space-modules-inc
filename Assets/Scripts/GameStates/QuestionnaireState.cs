using GameWork.Core.States.Tick.Input;

public class QuestionnaireState : InputTickState
{

	public const string StateName = "QuestionnaireState";

	public override string Name
	{
		get { return StateName; }
	}

	public QuestionnaireState(QuestionnaireStateInput input) : base(input)
	{
	}
}
