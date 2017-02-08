using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DropdownLocalization : MonoBehaviour {
	[SerializeField]
	private List<string> _options;

	private void OnEnable()
	{
		Localization.LanguageChange += OnLanguageChange;
		Set();
	}

	private void OnDisable()
	{
		Localization.LanguageChange -= OnLanguageChange;
	}

	public void SetOptions(List<string> options)
	{
		_options = options;
		Set();
	}

	private void Set()
	{
		var dropdown = GetComponent<Dropdown>();
		dropdown.ClearOptions();
		var translatedOptions = new List<string>();
		foreach (string t in _options)
		{
			translatedOptions.Add(Localization.Get(t));
		}
		dropdown.AddOptions(translatedOptions);
	}

	private void OnLanguageChange()
	{
		Set();
	}
}
