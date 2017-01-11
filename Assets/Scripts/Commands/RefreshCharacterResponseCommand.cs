using System.Diagnostics;
using GameWork.Core.Commands.Interfaces;

public class RefreshCharacterResponseCommand : ICommand<ScenarioController>
{

    public void Execute(ScenarioController parameter)
    {
        parameter.GetCharacterResponse();
    }
}
