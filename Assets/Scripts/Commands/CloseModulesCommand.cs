using GameWork.Core.Commands.Interfaces;

public class CloseModulesCommand : ICommand<ModulesController>
{
	public void Execute(ModulesController parameter)
	{
		parameter.ClosePopup();
	}
}