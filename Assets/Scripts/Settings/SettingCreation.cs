using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingCreation : MonoBehaviour
{
	[SerializeField]
	private Text _label;
	[SerializeField]
	private HorizontalLayoutGroup _horizontalLayout;
	[SerializeField]
	private VerticalLayoutGroup _verticalLayout;
	[SerializeField]
	private GameObject _dropdown;
	[SerializeField]
	private GameObject _toggle;
	[SerializeField]
	private GameObject _slider;
	[SerializeField]
	private GameObject _button;
	private TextAnchor _labelAnchor = TextAnchor.MiddleRight;
	private Resolution _previousResolution;

	private void OnValidate()
	{
		if (!_dropdown.GetComponent<Dropdown>() && !_dropdown.GetComponentInChildren<Dropdown>())
		{
			_dropdown = null;
		}
		if (!_toggle.GetComponent<Toggle>() && !_toggle.GetComponentInChildren<Toggle>())
		{
			_toggle = null;
		}
		if (!_slider.GetComponent<Slider>() && !_slider.GetComponentInChildren<Slider>())
		{
			_slider = null;
		}
		if (!_button.GetComponent<Button>() && !_button.GetComponentInChildren<Button>())
		{
			_button = null;
		}
	}

	private void OnEnable()
	{
		Localization.LanguageChange += OnChange;
		BestFit.ResolutionChange += OnChange;
	}

	private void OnDisable()
	{
		Localization.LanguageChange -= OnChange;
		BestFit.ResolutionChange -= OnChange;
	}

	private void Update()
	{
		if (_previousResolution.width != Screen.currentResolution.width || _previousResolution.height != Screen.currentResolution.height)
		{
			_previousResolution = Screen.currentResolution;
			RebuildLayout();
		}
	}

	public Dropdown Resolution(int minWidth, int minHeight, Resolution[] newResolutions = null, bool layoutHorizontal = true, string title = "Resolution", bool showOnMobile = false)
	{
		var newObj = Custom<Dropdown>(title, SettingObjectType.Dropdown, showOnMobile, layoutHorizontal);
		newObj.ClearOptions();
		var resolutions = Screen.resolutions.Distinct().ToList();
		if (newResolutions != null)
		{
			resolutions = resolutions.Concat(newResolutions).Distinct().ToList();
		}
		resolutions = resolutions.Where(res => res.width >= minWidth && res.height >= minHeight).ToList();
		var current = new Resolution { height = Screen.height, width = Screen.width };
		if (!resolutions.Any(res => res.width == current.width && res.height == current.height))
		{
			resolutions.Add(current);
		}
		var resolutionStrings = resolutions.OrderByDescending(res => res.width).ThenByDescending(res => res.height).Select(res => res.width + " x " + res.height).ToList();
		newObj.GetComponent<DropdownLocalization>().SetOptions(resolutionStrings);
		newObj.value = resolutionStrings.IndexOf(Screen.width + " x " + Screen.height);
		return newObj;
	}

	public Dropdown Quality(bool layoutHorizontal = true, string title = "Quality", bool showOnMobile = false)
	{
		var newObj = Custom<Dropdown>(title, SettingObjectType.Dropdown, showOnMobile, layoutHorizontal);
		newObj.ClearOptions();
		var qualities = QualitySettings.names.ToList();
		newObj.GetComponent<DropdownLocalization>().SetOptions(qualities);
		newObj.value = QualitySettings.GetQualityLevel();
		return newObj;
	}

	public Dropdown Language(bool layoutHorizontal = true, string title = "Language", bool showOnMobile = true)
	{
		var newObj = Custom<Dropdown>(title, SettingObjectType.Dropdown, showOnMobile, layoutHorizontal);
		newObj.ClearOptions();
		var languages = Localization.AvailableLanguages();
		newObj.GetComponent<DropdownLocalization>().SetOptions(languages);
		var selectedIndex = languages.IndexOf(Localization.SelectedLanguage.ToString());
		if (selectedIndex == -1)
		{
			var nullList = new List<string>{ string.Empty };
			newObj.AddOptions(nullList);
			newObj.value = languages.Count;
			newObj.options.RemoveAt(languages.Count);
		}
		else
		{
			newObj.value = selectedIndex;
		}
		
		return newObj;
	}

	public Toggle FullScreen(bool layoutHorizontal = true, string title = "FullScreen", bool showOnMobile = false)
	{
		var newObj = Custom<Toggle>(title, SettingObjectType.Toggle, showOnMobile, layoutHorizontal);
		newObj.isOn = Screen.fullScreen;
		return newObj;
	}

	public Slider Volume(string title, float defaultVolume = 1f, bool layoutHorizontal = true, bool showOnMobile = true)
	{
		var newObj = Custom<Slider>(title, SettingObjectType.Slider, showOnMobile, layoutHorizontal);
		if (!PlayerPrefs.HasKey(title))
		{
			PlayerPrefs.SetFloat(title, defaultVolume);
		}
		newObj.value = PlayerPrefs.GetFloat(title);
		return newObj;
	}

	public T Custom<T>(string label, SettingObjectType type, bool showOnMobile, bool layoutHorizontal = true) where T : UIBehaviour
	{
		var newObj = Create(new SettingType(label, type, showOnMobile), layoutHorizontal);
		var returnSelectable = newObj.GetComponent<T>() ?? newObj.GetComponentInChildren<T>();
		return returnSelectable;
	}

	public GameObject HorizontalLayout(string title, float aspectRatio = 0)
	{
		var layout = Instantiate(_horizontalLayout, transform, false);
		layout.name = title;
		if (aspectRatio > 0)
		{
			layout.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
		}
		return layout.gameObject;
	}

	public GameObject VerticalLayout(string title, float aspectRatio = 0)
	{
		var layout = Instantiate(_verticalLayout, transform, false);
		layout.name = title;
		if (aspectRatio > 0)
		{
			layout.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
		}
		return layout.gameObject;
	}

	public void AddToLayout(GameObject layoutObject, Selectable addition)
	{
		var layout = layoutObject.GetComponent<LayoutGroup>();
		var trans = addition.transform;
		if (layout)
		{
			AddToLayout(layout, trans);
		}
	}

	public void AddToLayout(GameObject layoutObject, Graphic addition)
	{
		var layout = layoutObject.GetComponent<LayoutGroup>();
		var trans = addition.transform;
		if (layout)
		{
			AddToLayout(layout, trans);
		}
	}

	private void AddToLayout(LayoutGroup layout, Transform addition)
	{
		var addObj = addition.gameObject.transform;
		while (addObj.transform.parent != null && addObj.transform.parent != transform)
		{
			addObj = addObj.parent;
		}
		addObj.SetParent(layout.transform, false);
	}

	public void SetLabelAlignment(TextAnchor align)
	{
		_labelAnchor = align;
	}

	private GameObject Create(SettingType setting, bool layoutHorizontal)
	{
		if (setting.ObjectType == SettingObjectType.Button)
		{
			var button = Instantiate(_button, transform, false);
			button.name = setting.Title;
			if (button.GetComponentInChildren<Text>())
			{
				button.GetComponentInChildren<Localization>().Key = setting.Title;
				button.GetComponentInChildren<Text>().text = Localization.Get(setting.Title);
			}
			if (!setting.ShowOnMobile && MobilePlatform.IsMobile())
			{
				button.gameObject.SetActive(false);
			}
			return button.gameObject;
		}

		var layout = layoutHorizontal ? HorizontalLayout(setting.Title) : VerticalLayout(setting.Title);
		var label = Instantiate(_label);
		AddToLayout(layout, label);
		label.name = _label.name;
		label.GetComponent<Localization>().Key = setting.Title;
		label.text = Localization.Get(setting.Title);
		label.alignment = _labelAnchor;
		if (!setting.ShowOnMobile && MobilePlatform.IsMobile())
		{
			layout.gameObject.SetActive(false);
		}
		switch (setting.ObjectType)
		{
			case SettingObjectType.Dropdown:
				var dropdown = Instantiate(_dropdown);
				var dropdownComp = dropdown.GetComponent<Dropdown>() ?? dropdown.GetComponentInChildren<Dropdown>();
				AddToLayout(layout, dropdownComp);
				var templateCanvas = dropdownComp.template.gameObject.AddComponent<Canvas>();
				templateCanvas.overrideSorting = false;
				var parentObject = transform.parent;
				var parentCanvas = parentObject.GetComponent<Canvas>();
				while (parentCanvas == null && parentObject != transform.root)
				{
					parentObject = parentObject.parent;
					parentCanvas = parentObject.GetComponent<Canvas>();
				}
				if (parentCanvas != null)
				{
					templateCanvas.sortingOrder = parentCanvas.sortingOrder;
					templateCanvas.sortingLayerName = parentCanvas.sortingLayerName;
				}
				dropdown.name = setting.Title;
				break;
			case SettingObjectType.Slider:
				var slider = Instantiate(_slider);
				AddToLayout(layout, slider.GetComponent<Slider>() ?? slider.GetComponentInChildren<Slider>());
				slider.name = setting.Title;
				break;
			case SettingObjectType.Toggle:
				var toggle = Instantiate(_toggle);
				AddToLayout(layout, toggle.GetComponent<Toggle>() ?? toggle.GetComponentInChildren<Toggle>());
				toggle.name = setting.Title;
				break;
            case SettingObjectType.Label:
                layout.GetComponent<LayoutElement>().preferredHeight *= 1.5f;
                layout.GetComponent<AspectRatioFitter>().aspectRatio /= 1.5f;
                break;

        }

        return layout.gameObject;
	}

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) transform);

        foreach (LayoutGroup layout in GetComponentsInChildren<LayoutGroup>())
        {
            if (layout.gameObject != gameObject)
            {
                foreach (Transform trans in layout.transform)
                {
                    var aspect = trans.GetComponent<AspectRatioFitter>() ??
                                 trans.gameObject.AddComponent<AspectRatioFitter>();
                    var layoutElement = trans.GetComponent<LayoutElement>() ??
                                        trans.gameObject.AddComponent<LayoutElement>();
                    aspect.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                    var canvasSize = ((RectTransform) GetComponentInParent<Canvas>().transform).rect.size;
                    if (layout.GetComponent<RectTransform>().sizeDelta.x > canvasSize.x)
                    {
                        layout.GetComponent<LayoutElement>().preferredHeight = canvasSize.x /
                                                                               layout.GetComponent<AspectRatioFitter>()
                                                                                   .aspectRatio;
                    }
                    if (layout.GetType() == typeof(HorizontalLayoutGroup))
                    {
                        layoutElement.preferredHeight = layout.GetComponent<LayoutElement>().preferredHeight;
                        aspect.aspectRatio = layout.GetComponent<AspectRatioFitter>().aspectRatio /
                                             layout.transform.childCount;
                    }
                    else
                    {
                        layoutElement.preferredHeight = layout.GetComponent<LayoutElement>().preferredHeight /
                                                        layout.transform.childCount;
                        aspect.aspectRatio = layout.GetComponent<AspectRatioFitter>().aspectRatio *
                                             layout.transform.childCount;
                    }
                }
            }
        }

        gameObject.BestFit();
        foreach (LayoutGroup layout in GetComponentsInChildren<LayoutGroup>())
        {
            if (layout.gameObject != gameObject)
            {
                if (layout.transform.childCount == 1 && layout.transform.GetChild(0).GetComponent<Text>())
                {
                    layout.transform.GetChild(0).GetComponent<Text>().fontSize =
                        (int) (layout.transform.GetChild(0).GetComponent<Text>().fontSize * 1.5f);
                }
            }
        }
    }

    public void Wipe()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}

	private void OnChange()
	{
		RebuildLayout();
	}
}
