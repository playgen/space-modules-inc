using GameWork.Core.Commands.Interfaces;

public class RefreshLevelDataCommand : ICommand<ScenarioController>
{
    public void Execute(ScenarioController parameter)
    {
        parameter.RefreshLevelData();
    }
}