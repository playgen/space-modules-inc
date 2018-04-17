using System;
using GameWork.Core.Commands.Interfaces;

public class SetPlayerActionCommand : ICommand<ScenarioController>
{
    private readonly Guid _actionId;

    public SetPlayerActionCommand(Guid actionId)
    {
        _actionId = actionId;
    }

    public void Execute(ScenarioController parameter)
    {
        parameter.SetPlayerAction(_actionId);
    }
}
