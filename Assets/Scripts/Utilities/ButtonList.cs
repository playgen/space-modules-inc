using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ButtonList
{
	private readonly GameObject[] _buttons;

	public GameObject[] GameObjects
	{
		get
		{
			return _buttons;
		}
	}

	public ButtonList(string menuPath)
	{
		_buttons = GameObjectUtilities.FindAllChildren(menuPath);
	}

	public Button GetButton(string buttonName, bool hasContainer = false)
	{
		var button = _buttons.First(o => o.name.Equals(buttonName));
		return !hasContainer ? button.GetComponent<Button>() : button.GetComponentInChildren<Button>();
	}
}
