using Assets.Scripts.Inputs;
using GameWork.Core.States.Tick.Input;

public class SettingsState : InputTickState
{
    private readonly SettingsStateInput _input;
    public const string StateName = "SettingsState";

    public SettingsState(SettingsStateInput input) : base(input)
    {
    }

    public override string Name
    {
        get { return StateName; }
    }
}
