using GameWork.Core.Commands.Interfaces;

public class SetLevelCommand : ICommand<ScenarioController>
{
    private readonly int _id;

    public SetLevelCommand(int id)
    {
        _id = id;
    }

    public void Execute(ScenarioController parameter)
    {
        parameter.SetLevel(_id);
    }
}
