using GameWork.Commands.Interfaces;

public class RefreshLevelDataCommand : ICommand<ScenarioController>
{
    public void Execute(ScenarioController parameter)
    {
        parameter.RefreshCharacterArray();
    }
}