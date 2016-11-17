using GameWork.Core.Commands.Interfaces;

public class GetReviewDataCommand : ICommand<ScenarioController>
{
    public void Execute(ScenarioController implementor)
    {
        implementor.GetReviewData();
    }
}
