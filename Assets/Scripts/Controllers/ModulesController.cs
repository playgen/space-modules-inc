﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameWork.Core.Commands.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ModulesController : ICommandAction
{
	#region Modules Config

	struct ModulesDatabase
	{
		public ModuleEntry[] Database;
	}

	struct ModuleEntry
	{
		public string Name;
		public string Id;
		public string Type;
		public string Icon;
		public string Description;
		public FaqEntry[] Faq;
	}

	struct FaqEntry
	{
		public string Problem;
		public string Solution;
	}

	#endregion

	private readonly GameObject _modulesPopup;
    private readonly GameObject _popupContent;
	private readonly GameObject _backgroundOverlay;
	private readonly GameObject _moduleItemPrefab;
    private readonly GameObject _moduleIndexItemPrefab;
    private readonly GameObject _moduleDescriptionItemPrefab;
    private readonly RectTransform _modulesContent;

    private readonly Sprite[] _moduleIcons;
    private int _stateLevel;
    private ModuleEntry[] _modulesDatabase;
    private Button _backButton;

	public ModulesController()
    {
        _modulesPopup = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup");
        _backgroundOverlay = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer");
        _popupContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View");
        _modulesContent = _popupContent.GetComponent<ScrollRect>().content;
        
        _moduleItemPrefab = Resources.Load("Prefabs/ModuleItem") as GameObject;
        _moduleIndexItemPrefab = Resources.Load("Prefabs/ModuleIndexItem") as GameObject;
        _moduleDescriptionItemPrefab = Resources.Load("Prefabs/ModuleDescriptionItem") as GameObject;
        _moduleIcons = Resources.LoadAll<Sprite>("Sprites/Modules/Icons");

        var backButtonObject = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackButton");
        _backButton = backButtonObject.GetComponent<Button>();

		_backgroundOverlay.GetComponent<Button>().onClick.AddListener(TogglePopup);

		LoadJSONModules();
	}

    private void LoadIndex()
    {
        ClearList();
        var moduleTypes = _modulesDatabase.Select(entry => entry.Type).Distinct().ToArray();

        for (var i = 0; i < moduleTypes.Length; i++)
        {
            var moduleType = moduleTypes[i];
            var listItem = InstantiateListItem(_moduleIndexItemPrefab);
	        var iconId = _modulesDatabase.FirstOrDefault(entry => entry.Type.Equals(moduleType)).Icon;
            listItem.transform.FindChild("Text").GetComponent<Text>().text = moduleType;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(iconId));

            var offset = i * listItem.GetComponent<RectTransform>().rect.height;
            listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
            listItem.GetComponent<Button>().onClick.AddListener(delegate
            {
				LoadModules(moduleType);
			});
        }
        
        _backButton.onClick.AddListener(TogglePopup);

    }

    private void LoadModules(string moduleTypeName)
    {
		
		ClearList();

        var modules = _modulesDatabase.Where(entry => entry.Type.Equals(moduleTypeName)).ToArray();

        for (var i = 0; i < modules.Length; i++)
        {
            var module = modules[i];
            var listItem = InstantiateListItem(_moduleItemPrefab);

            listItem.transform.FindChild("Text").GetComponent<Text>().text = module.Name;
            listItem.transform.FindChild("Id").GetComponent<Text>().text = module.Id;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(module.Icon));

            var offset = i * listItem.GetComponent<RectTransform>().rect.height;
            listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
            listItem.GetComponent<Button>().onClick.AddListener(delegate
            {
				LoadModule(module, listItem);
            });
        }
        _backButton.onClick.AddListener(LoadIndex);

    }

    private void LoadModule(ModuleEntry module, GameObject listItem)
    {
        _modulesContent.GetComponentInChildren<VerticalLayoutGroupCustom>().enabled = true;
        var moduleItem = GameObject.Instantiate(listItem);
        ClearList();
        moduleItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
        moduleItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        foreach (var description in module.Faq)
        {
            var problemItem = InstantiateListItem(_moduleDescriptionItemPrefab);
			problemItem.transform.FindChild("Title").GetComponent<Text>().text = Localization.Get("PROBLEM", true);
	        var problemText = description.Problem + String.Format("\n\n<color=#5BEAFFFF><b>{0}</b>:</color>\n{1}\n", Localization.Get("SOLUTION"), description.Solution);
			problemItem.transform.FindChild("Panel").GetChild(0).GetComponent<Text>().text = problemText;
		}
		_backButton.onClick.AddListener(delegate
        {
			LoadModules(module.Type);
		});
    }


    private void ClearList()
    {
        _backButton.onClick.RemoveAllListeners();
        foreach (RectTransform child in _popupContent.GetComponent<ScrollRect>().content)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void LoadJSONModules()
    {
		var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "modules.json");
		var streamReader = new StreamReader(streamingAssetsPath);
		_modulesDatabase = JsonConvert.DeserializeObject<ModulesDatabase>(streamReader.ReadToEnd()).Database;
	}



	private GameObject InstantiateListItem(GameObject prefab)
    {
        var listItem = GameObject.Instantiate(prefab);
        listItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
		return listItem;
    }

    public void TogglePopup()
    {
		if (_modulesPopup.activeInHierarchy)
        {
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = false;
			_modulesPopup.SetActive(false);

		}
		else
        {
            _modulesPopup.SetActive(true);
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = true;
			_popupContent.GetComponent<ScrollRect>().verticalScrollbar.enabled = true;
			LoadIndex();
        }
    }
}
