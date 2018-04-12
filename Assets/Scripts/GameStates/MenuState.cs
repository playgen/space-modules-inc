using GameWork.Core.States.Tick.Input;

public class MenuState : InputTickState
{
	public const string StateName = "MenuState";

	public override string Name => StateName;

	public MenuState(MenuStateInput input) : base(input)
	{
	}
}
