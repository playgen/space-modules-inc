using GameWork.Core.Commands.Interfaces;
using UnityEngine;

public class UpdateDialogueFontSizeCommand : ICommand
{
	private readonly GameObject _dialogueObject;

	public UpdateDialogueFontSizeCommand(GameObject dialogueGameObject)
	{
		_dialogueObject = dialogueGameObject;
	}

	public void Execute(GameStateInput parameter)
	{
		parameter.ResizeOptions(_dialogueObject);
	}
}