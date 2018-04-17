using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ButtonList
{
	public GameObject[] GameObjects { get; }

	public ButtonList(string menuPath)
	{
		GameObjects = GameObjectUtilities.FindAllChildren(menuPath);
	}

	public Button GetButton(string buttonName, bool hasContainer = false)
	{
		var button = GameObjects.First(o => o.name.Equals(buttonName));
		return !hasContainer ? button.GetComponent<Button>() : button.GetComponentInChildren<Button>();
	}
}
