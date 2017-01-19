using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameWork.Core.Commands.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
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
	private Button _nextArrow;
	private Button _backArrow;

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

		var nextArrowObject = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/NextArrow");
	    _nextArrow = nextArrowObject.GetComponent<Button>();

		var backArrowObject = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackArrow");
	    _backArrow = backArrowObject.GetComponent<Button>();


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
				_nextArrow.gameObject.SetActive(true);
				_backArrow.gameObject.SetActive(true);
			});
        }
        _backButton.onClick.AddListener(LoadIndex);

    }

    private void LoadModule(ModuleEntry module, GameObject listItem)
    {
	    var currentModuleList = module.Faq;
	    var index = 0;
	    GameObject[] moduleProblem;

        _modulesContent.GetComponentInChildren<VerticalLayoutGroupCustom>().enabled = true;
        var moduleItem = GameObject.Instantiate(listItem);
        ClearList();
        moduleItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
        moduleItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


		// TODO: Renable me for module descriptions
	    //var descriptionItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		//descriptionItem.transform.FindChild("Title").GetComponent<Text>().text = Localization.Get("DESCRIPTION", true);
		//descriptionItem.transform.FindChild("Panel").GetChild(0).GetComponent<Text>().text = module.Description;

	    moduleProblem = ShowModuleProblem(currentModuleList[index]);

		// TODO: Refactor this
		_nextArrow.onClick.AddListener(() =>
		{
			ClearModuleProblem(moduleProblem);
			index++;
			if (index == currentModuleList.Length)
			{
				index = 0;
			}
			moduleProblem = ShowModuleProblem(currentModuleList[index]);
		});

		_backArrow.onClick.AddListener(() =>
		{
			ClearModuleProblem(moduleProblem);
			index--;
			if (index < 0)
			{
				index = currentModuleList.Length - 1;
			}
			moduleProblem = ShowModuleProblem(currentModuleList[index]);
		});

		_backButton.onClick.AddListener(delegate
		{
			_nextArrow.onClick.RemoveAllListeners();
			_backArrow.onClick.RemoveAllListeners();
			_nextArrow.gameObject.SetActive(false);
			_backArrow.gameObject.SetActive(false);
			LoadModules(module.Type);
		});

	}

	private void ClearModuleProblem(GameObject[] moduleProblem)
	{
		foreach (var gameObject in moduleProblem)
		{
			GameObject.Destroy(gameObject);
		}
	}

	private GameObject[] ShowModuleProblem(FaqEntry currentModule)
	{
		var problemItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		problemItem.transform.FindChild("Title").GetComponent<Text>().text = Localization.Get("PROBLEM", true);
		problemItem.transform.FindChild("Panel").GetChild(0).GetComponent<Text>().text = currentModule.Problem;
		var solutionItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		solutionItem.transform.FindChild("Title").GetComponent<Text>().text = Localization.Get("SOLUTION", true);
		solutionItem.transform.FindChild("Panel").GetChild(0).GetComponent<Text>().text = currentModule.Solution;

		return new[] {problemItem, solutionItem};
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
			_nextArrow.onClick.RemoveAllListeners();
			_backArrow.onClick.RemoveAllListeners();
			_nextArrow.gameObject.SetActive(false);
			_backArrow.gameObject.SetActive(false);
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
