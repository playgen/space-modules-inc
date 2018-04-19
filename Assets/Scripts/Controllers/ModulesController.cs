using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameWork.Core.Commands.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

using TrackerAssetPackage;

public class ModulesController : ICommandAction
{
	#region Modules Config

	private struct ModulesDatabase
	{
		public ModuleEntry[] Database;
	}

	private struct ModuleEntry
	{
		public string Name;
		public string Id;
		public string Type;
		public string Icon;
		public string Description;
		public FaqEntry[] Faq;
	}

	private struct FaqEntry
	{
		public string Problem;
		public string Solution;
	}

	#endregion

	private readonly GameObject _modulesPopup;
	private readonly GameObject _popupContent;

	private readonly GameObject _moduleItemPrefab;
	private readonly GameObject _moduleIndexItemPrefab;
	private readonly GameObject _moduleDescriptionItemPrefab;
	private readonly RectTransform _modulesContent;

	private readonly Sprite[] _moduleIcons;
	private ModuleEntry[] _modulesDatabase;
	private readonly Button _backButton;
	private readonly Button _nextArrow;
	private readonly Button _backArrow;

	public ModulesController()
	{
		var aotFaq = new FaqEntry ();
		var aotFaqList = new List<FaqEntry> ();
		var aotModEnt = new ModuleEntry ();
		var aotModEntList = new List<ModuleEntry> ();
		var aotModDat = new ModulesDatabase ();
		var aotModDatList = new List<ModulesDatabase> ();

		_modulesPopup = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup");
		var backgroundOverlay = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer");
		_popupContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View");
		_modulesContent = _popupContent.GetComponent<ScrollRect>().content;
		
		_moduleItemPrefab = Resources.Load("Prefabs/ModuleItem") as GameObject;
		_moduleIndexItemPrefab = Resources.Load("Prefabs/ModuleIndexItem") as GameObject;
		_moduleDescriptionItemPrefab = Resources.Load("Prefabs/ModuleDescriptionItem") as GameObject;
		_moduleIcons = Resources.LoadAll<Sprite>("Sprites/Modules/Icons");

		var backButtonObject = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackButton");
		_backButton = backButtonObject.GetComponent<Button>();

		backgroundOverlay.GetComponent<Button>().onClick.AddListener(delegate
		{
			TogglePopup();
			TrackerEventSender.SendEvent(new TraceEvent("CloseModulePopUp", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});

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
			listItem.transform.Find("Text").GetComponent<Text>().text = moduleType;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(iconId));

			var offset = i * listItem.GetComponent<RectTransform>().rect.height;
			listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
			listItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				LoadModules(moduleType);
				TrackerEventSender.SendEvent(new TraceEvent("Device." + moduleType.Replace(" ", "_"), TrackerAsset.Verb.Accessed, new Dictionary<string, string>
				{

				}, AccessibleTracker.Accessible.Screen));
			});
		}

		_backButton.GetComponent<Button>().onClick.AddListener(delegate
		{
			TogglePopup();
			TrackerEventSender.SendEvent(new TraceEvent("CloseModulePopUp", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});
	}

	private void LoadModules(string moduleTypeName)
	{
		ClearList();
		var modules = _modulesDatabase.Where(entry => entry.Type.Equals(moduleTypeName)).ToArray();

		for (var i = 0; i < modules.Length; i++)
		{
			var module = modules[i];
			var listItem = InstantiateListItem(_moduleItemPrefab);

			listItem.transform.Find("Text").GetComponent<Text>().text = module.Name;
			listItem.transform.Find("Id").GetComponent<Text>().text = module.Id;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(module.Icon));

			var offset = i * listItem.GetComponent<RectTransform>().rect.height;
			listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
			listItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				LoadModule(module, listItem);
				_nextArrow.gameObject.SetActive(true);
				_backArrow.gameObject.SetActive(true);
				TrackerEventSender.SendEvent(new TraceEvent("Device." + moduleTypeName.Replace(" ", "_") + "." + module.Id, TrackerAsset.Verb.Accessed, new Dictionary<string, string>
				{

				}, AccessibleTracker.Accessible.Screen));
			});
		}
		_backButton.GetComponent<Button>().onClick.AddListener(delegate
		{
			LoadIndex();
			TrackerEventSender.SendEvent(new TraceEvent("BackToModuleDevices", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});
	}

	private void LoadModule(ModuleEntry module, GameObject listItem)
	{
		var currentModuleList = module.Faq;
		var index = 0;

		var moduleItem = Object.Instantiate(listItem);
		ClearList();
		moduleItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
		moduleItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

		var problemItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		problemItem.transform.Find("Title").GetComponent<Text>().text = Localization.Get("PROBLEM", true);
		var problemItemText = problemItem.transform.GetChild(2).GetComponent<Text>();
		problemItemText.text = currentModuleList[0].Problem;
		problemItem.GetComponent<ContentSizeFitterHelper>().Action = Rebuild;

		var solutionItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		solutionItem.transform.Find("Title").GetComponent<Text>().text = Localization.Get("SOLUTION", true);
		var solutionItemText = solutionItem.transform.GetChild(2).GetComponent<Text>();
		solutionItemText.text = currentModuleList[0].Solution;
		solutionItem.GetComponent<ContentSizeFitterHelper>().Action = Rebuild;

		// Step through arrow listeners
		_nextArrow.onClick.AddListener(() =>
		{
			index++;
			if (index == currentModuleList.Length)
			{
				index = 0;
			}
			problemItemText.text = currentModuleList[index].Problem;
			solutionItemText.text = currentModuleList[index].Solution;
			TrackerEventSender.SendEvent(new TraceEvent("NextModuleFAQ", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});

		_backArrow.onClick.AddListener(() =>
		{
			index--;
			if (index < 0)
			{
				index = currentModuleList.Length - 1;
			}
			problemItemText.text = currentModuleList[index].Problem;
			solutionItemText.text = currentModuleList[index].Solution;
			TrackerEventSender.SendEvent(new TraceEvent("PreviousModuleFAQ", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});

		_backButton.onClick.AddListener(delegate
		{
			_nextArrow.onClick.RemoveAllListeners();
			_backArrow.onClick.RemoveAllListeners();
			_nextArrow.gameObject.SetActive(false);
			_backArrow.gameObject.SetActive(false);
			LoadModules(module.Type);
			TrackerEventSender.SendEvent(new TraceEvent("BackToModuleDeviceTypes", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
		});
	}

	private void Rebuild()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(_modulesContent);
	}

	private void ClearList()
	{
		_backButton.onClick.RemoveAllListeners();
		foreach (RectTransform child in _popupContent.GetComponent<ScrollRect>().content)
		{
			Object.Destroy(child.gameObject);
		}
	}

	private void LoadJSONModules()
	{
		var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "modules.json");
		var www = new WWW((Application.platform != RuntimePlatform.Android ? "file:///" : string.Empty) + streamingAssetsPath);
		while (!www.isDone)
		{
		}
		_modulesDatabase = JsonConvert.DeserializeObject<ModulesDatabase>(www.text).Database;
	}

	private GameObject InstantiateListItem(GameObject prefab)
	{
		var listItem = Object.Instantiate(prefab);
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
			TrackerEventSender.SendEvent(new TraceEvent("ModuleList", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{

			}, AccessibleTracker.Accessible.Screen));
			_modulesPopup.SetActive(true);
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = true;
			_popupContent.GetComponent<ScrollRect>().verticalScrollbar.enabled = true;
			LoadIndex();
		}
	}
}
