using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemBehaviour : MonoBehaviour
{
    [SerializeField]
    private Sprite _starSprite;

    [SerializeField]
    private Text _itemText;

    [SerializeField]
    private GameObject[] _starSlots;

    public void SetupItem(int stars, string itemName)
    {
        if (stars > _starSlots.Length)
        {
            throw new Exception("stars & slots mismatch");
        }

        _itemText.text = itemName;
        for (var i = 0; i < stars; i++)
        {
            _starSlots[i].GetComponent<Image>().sprite = _starSprite;
        }
    }
}
