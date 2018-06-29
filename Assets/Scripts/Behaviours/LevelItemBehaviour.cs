using UnityEngine;
using UnityEngine.UI;

public class LevelItemBehaviour : MonoBehaviour
{
	[SerializeField]
	private Sprite _starSprite;

	[SerializeField]
	private Text _itemText;

	[SerializeField]
	private Image[] _starSlots;

	public void SetupItem(int stars, string itemName)
	{
		_itemText.text = itemName;
		for (var i = 0; i < _starSlots.Length; i++)
		{
			if (stars <= i)
			{
				return;
			}
			_starSlots[i].sprite = _starSprite;
		}
	}
}
