using GameWork.Commands.Interfaces;

public class SetLevelCommand : ICommand<ScenarioController>
{
    private readonly string _name;

    public SetLevelCommand(string name)
    {
        _name = name;
    }

    public void Execute(ScenarioController parameter)
    {
        parameter.SetCharacter(_name);
    }
}
