using System;
using GameWork.Commands.Interfaces;

public class SetPlayerActionCommand : ICommand<ScenarioController>
{
    private Guid _actionId;

    public SetPlayerActionCommand(Guid actionId)
    {
        _actionId = actionId;
    }

    public void Execute(ScenarioController parameter)
    {
        parameter.SetPlayerAction(_actionId);
    }
}
