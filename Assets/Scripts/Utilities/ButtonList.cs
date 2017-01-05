using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ButtonList
{
    private readonly GameObject[] _buttons;

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

    public void BestFit()
    {
        if (_buttons != null && _buttons.Length > 0)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_buttons[0].transform.parent);
        }
        int smallestFontSize = 0;
        foreach (var textObj in _buttons)
        {
            var text = textObj.GetComponentInChildren<Text>();
            if (!text)
            {
                continue;
            }
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 1;
            text.resizeTextMaxSize = 100;
            text.cachedTextGenerator.Invalidate();
            text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
            text.resizeTextForBestFit = false;
            var newSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
            var newSizeRescale = text.rectTransform.rect.size.x / text.cachedTextGenerator.rectExtents.size.x;
            if (text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y < newSizeRescale)
            {
                newSizeRescale = text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y;
            }
            newSize = Mathf.FloorToInt(newSize * newSizeRescale);
            if (newSize < smallestFontSize || smallestFontSize == 0)
            {
                smallestFontSize = newSize;
            }
        }
        foreach (var textObj in _buttons)
        {
            var text = textObj.GetComponentInChildren<Text>();
            if (!text)
            {
                continue;
            }
            text.fontSize = smallestFontSize;
        }
    }

    public Button GetButton(string buttonName, bool hasContainer = false)
    {
        var button = _buttons.First(o => o.name.Equals(buttonName));
        if (!hasContainer)
        {
            return button.GetComponent<Button>();
        }
        else
        {
            return button.transform.GetChild(0).GetComponent<Button>();
        }
    }
}
