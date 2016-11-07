using GameWork.Commands.Interfaces;

public class RefreshPlayerDialogueCommand : ICommand<ScenarioController>
{
    public void Execute(ScenarioController parameter)
    {
        parameter.GetPlayerDialogueOptions();   
    }
}
