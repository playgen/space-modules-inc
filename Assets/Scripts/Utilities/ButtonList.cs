using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ButtonList
{
    private GameObject[] _buttons;

    public ButtonList(string menuPath, bool standardBestFit = false)
    {
        _buttons = GameObjectUtilities.FindAllChildren(menuPath);
        if (standardBestFit)
        {
            int smallestFontSize = 0;
            foreach (var button in _buttons)
            {
                button.GetComponentInChildren<Text>().resizeTextForBestFit = true;
                button.GetComponentInChildren<Text>().cachedTextGenerator.Populate(button.GetComponentInChildren<Text>().text, button.GetComponentInChildren<Text>().GetGenerationSettings(button.GetComponentInChildren<Text>().rectTransform.rect.size));
                button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
                if (button.GetComponentInChildren<Text>().cachedTextGenerator.fontSizeUsedForBestFit < smallestFontSize || smallestFontSize == 0)
                {
                    smallestFontSize = button.GetComponentInChildren<Text>().cachedTextGenerator.fontSizeUsedForBestFit;
                }
            }
            foreach (var button in _buttons)
            {
                button.GetComponentInChildren<Text>().fontSize = smallestFontSize;
            }
        }
    }

    public Button GetButton(string containerName)
    {
        return _buttons.First(o => o.name.Equals(containerName)).transform.GetChild(0).GetComponent<Button>();
    }
}
