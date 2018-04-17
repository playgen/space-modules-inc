using GameWork.Core.Commands.Interfaces;

public class GetScoreDataCommand : ICommand<ScenarioController>
{
    public void Execute(ScenarioController implementor)
    {
        implementor.GetScoreData();
    }
}
