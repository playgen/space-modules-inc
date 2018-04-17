using GameWork.Core.Commands.Interfaces;

public class ToggleModulesCommand : ICommand<ModulesController>
{
    public void Execute(ModulesController parameter)
    {
        parameter.TogglePopup();
    }
}
