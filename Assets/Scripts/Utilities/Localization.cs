using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;
using System;
using System.Linq;

public enum Language
{
	English = 1,
	//AmericanEnglish,
	//French,
	//Spanish,
	//Italian,
	//German,
	Dutch,
	//Greek,
	//Japanese,
	//ChineseSimplified
}

public class Localization : MonoBehaviour
{
	private static readonly Dictionary<Language, Dictionary<string, string>> LocalizationDict = new Dictionary<Language, Dictionary<string, string>>();

	public string Key;
	public bool ToUpper;

	public static string filePath = "StringLocalizations";
	public static Language SelectedLanguage { get; set; }
	public static event Action LanguageChange = delegate { };

	void Awake()
	{
		if (SelectedLanguage != 0)
		{
			return;
		}
		TextAsset jsonTextAsset = Resources.Load(filePath) as TextAsset;

		var N = JSON.Parse(jsonTextAsset.text);
		foreach (Language l in Enum.GetValues(typeof(Language)))
		{
			Dictionary<string, string> languageStrings = new Dictionary<string, string>();
			for (int i = 0; i < N.Count; i++)
			{
				//go through the list and add the strings to the dictionary
				string _key = N[i][0].ToString();
				_key = _key.Replace("\"", "").ToUpper();
				if (N[i][l.ToString()] != null)
				{
					string _value = N[i][l.ToString()].ToString();
					_value = _value.Replace("\"", "");
					languageStrings[_key] = _value;
				}
			}
			LocalizationDict[l] = languageStrings;
		}
		GetSystemLanguage();
	}

	void OnEnable()
	{
		Set();
	}

	public static string Get(string key, bool toUpper = false)
	{
		if (string.IsNullOrEmpty(key))
		{
			return string.Empty;
		}
		string txt;
		var newKey = key.ToUpper();
		newKey = newKey.Replace('-', '_');

		LocalizationDict[SelectedLanguage].TryGetValue(newKey, out txt);
		if (txt == null)
		{
			Debug.LogError("Could not find string with key: " + key);
			txt = key;
		}
		//new line character in spreadsheet is *n*
		txt = txt.Replace("\\n", "\n");
		if (toUpper)
		{
			txt = txt.ToUpper();
		}
		return txt;
	}

	public static string GetAndFormat(string key, bool toUpper, params object[] args)
	{
		return string.Format(Get(key, toUpper), args);
	}

	public void Set()
	{
		Text _text = GetComponent<Text>();
		if (_text == null)
		{
			Debug.LogError("Localization script could not find Text component attached to this gameObject: " + gameObject.name);
			return;
		}
		_text.text = Get(Key, ToUpper);
	}

	private void GetSystemLanguage()
	{
		switch (Application.systemLanguage)
		{
			case SystemLanguage.English:
				SelectedLanguage = Language.English;
				return;
			//case SystemLanguage.French:
			//	SelectedLanguage = Language.French;
			//	return;
			//case SystemLanguage.Spanish:
			//	SelectedLanguage = Language.Spanish;
			//	return;
			//case SystemLanguage.Italian:
			//	SelectedLanguage = Language.Italian;
			//	return;
			//case SystemLanguage.German:
			//	SelectedLanguage = Language.German;
			//	return;
			case SystemLanguage.Dutch:
				SelectedLanguage = Language.Dutch;
				return;
			//case SystemLanguage.Greek:
			//	SelectedLanguage = Language.Greek;
			//	return;
			//case SystemLanguage.Japanese:
			//	SelectedLanguage = Language.Japanese;
			//	return;
			//case SystemLanguage.ChineseSimplified:
			//	SelectedLanguage = Language.ChineseSimplified;
			//	return;
			default:
				SelectedLanguage = Language.English;
				return;
		}
	}

	public static void UpdateLanguage(Language language)
	{
		SelectedLanguage = language;
		((Localization[])FindObjectsOfType(typeof(Localization))).ToList().ForEach(l => l.Set());
		LanguageChange();
	}
}
