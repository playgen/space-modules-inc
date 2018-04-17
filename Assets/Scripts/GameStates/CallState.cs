using GameWork.Core.States.Tick.Input;

public class CallState : InputTickState
{
    public const string StateName = "CallState";

    public CallState(CallStateInput input) : base(input)
    {
    }

    public override string Name => StateName;
}
