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

	public struct ModuleEntry
	{
		public string Name;
		public string Id;
		public string Type;
		public string Icon;
		public string Description;
		public FaqEntry[] Faq;
	}

	public struct FaqEntry
	{
		public string Problem;
		public string Solution;
	}

	#endregion

	private readonly GameObject _modulesPopup;

	private readonly GameObject _moduleItemPrefab;
	private readonly GameObject _moduleIndexItemPrefab;
	private readonly GameObject _moduleDescriptionItemPrefab;
	private readonly RectTransform _modulesContent;

	private readonly Sprite[] _moduleIcons;
	private readonly Dictionary<string, ModuleEntry[]> _modulesDatabase = new Dictionary<string, ModuleEntry[]>();
	private readonly Button _backButton;
	private readonly Button _nextArrow;
	private readonly Button _backArrow;

	public ModulesController()
	{
		var aotFaqList = new List<FaqEntry>();
		var aotModEntList = new List<ModuleEntry>();
		Localization.Get("Start");

		_modulesPopup = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup");
		_modulesContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View").GetComponent<ScrollRect>().content;
		
		_moduleItemPrefab = Resources.Load("Prefabs/ModuleItem") as GameObject;
		_moduleIndexItemPrefab = Resources.Load("Prefabs/ModuleIndexItem") as GameObject;
		_moduleDescriptionItemPrefab = Resources.Load("Prefabs/ModuleDescriptionItem") as GameObject;
		_moduleIcons = Resources.LoadAll<Sprite>("Sprites/Modules/Icons");

		_backButton = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackButton").GetComponent<Button>();
		_nextArrow = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/NextArrow").GetComponent<Button>();
		_backArrow = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackArrow").GetComponent<Button>();

		GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer").GetComponent<Button>().onClick.AddListener(() =>
		{
			TogglePopup();
			SendTrackerEvents("ClosedModuleMenu", "ClosedModuleMenu");
		});

		foreach (var lang in Localization.Languages)
		{
			var www = new WWW((Application.platform != RuntimePlatform.Android ? "file:///" : string.Empty) + Path.Combine(Application.streamingAssetsPath, "modules-" + lang.Name + ".json"));
			while (!www.isDone)
			{
			}
			_modulesDatabase.Add(lang.Name, JsonConvert.DeserializeObject<ModuleEntry[]>(www.text));
		}
		TrackerEventSender.SetModuleDatabase(_modulesDatabase[Localization.SelectedLanguage.Name]);
	}

	private void LoadIndex()
	{
		ClearList();
		var moduleTypes = _modulesDatabase[Localization.SelectedLanguage.Name].Select(entry => entry.Type).Distinct().ToArray();

		foreach (var moduleType in moduleTypes)
		{
			var listItem = InstantiateListItem(_moduleIndexItemPrefab);
			var iconId = _modulesDatabase[Localization.SelectedLanguage.Name].FirstOrDefault(entry => entry.Type.Equals(moduleType)).Icon;
			listItem.transform.Find("Text").GetComponent<Text>().text = moduleType;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(iconId));
			listItem.GetComponent<Button>().onClick.AddListener(() =>
			{
				LoadModules(moduleType);
				SendTrackerEvents("SelectedModuleType", "SelectedModuleType", moduleType);
			});
		}

		_backButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			TogglePopup();
			SendTrackerEvents("CloseModuleMenu", "CloseModuleMenu");
		});
	}

	private void LoadModules(string moduleTypeName)
	{
		ClearList();
		_nextArrow.gameObject.SetActive(false);
		_backArrow.gameObject.SetActive(false);
		var modules = _modulesDatabase[Localization.SelectedLanguage.Name].Where(entry => entry.Type.Equals(moduleTypeName)).ToArray();

		foreach (var module in modules)
		{
			var listItem = InstantiateListItem(_moduleItemPrefab);

			listItem.transform.Find("Text").GetComponent<Text>().text = module.Name;
			listItem.transform.Find("Id").GetComponent<Text>().text = module.Id;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(module.Icon));
			listItem.GetComponent<Button>().onClick.AddListener(() =>
			{
				LoadModule(module, listItem);
				SendTrackerEvents("SelectedModule", "SelectedModule", module.Type, module.Id);
			});
		}
		_backButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			LoadIndex();
			SendTrackerEvents("BackToModuleTypeList", "BackToModuleTypeList");
		});
	}

	private void LoadModule(ModuleEntry module, GameObject listItem)
	{
		ClearList();
		_nextArrow.onClick.RemoveAllListeners();
		_backArrow.onClick.RemoveAllListeners();
		_nextArrow.gameObject.SetActive(true);
		_backArrow.gameObject.SetActive(true);
		var currentModuleList = module.Faq;
		var index = 0;

		InstantiateListItem(listItem);
		var problemItemText = InstantiateTextItem("PROBLEM", currentModuleList[0].Problem);
		var solutionItemText = InstantiateTextItem("SOLUTION", currentModuleList[0].Solution);

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
			SendTrackerEvents("NextModuleFAQ", "ViewNextProblem", module.Type, module.Id);
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
			SendTrackerEvents("PreviousModuleFAQ", "ViewPreviousProblem", module.Type, module.Id);
		});

		_backButton.onClick.AddListener(() =>
		{
			LoadModules(module.Type);
			SendTrackerEvents("BackToModuleDeviceTypes", "BackToModuleList");
		});
	}

	private void Rebuild()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(_modulesContent);
	}

	private void ClearList()
	{
		_backButton.onClick.RemoveAllListeners();
		foreach (RectTransform child in _modulesContent)
		{
			Object.Destroy(child.gameObject);
		}
	}

	private GameObject InstantiateListItem(GameObject prefab)
	{
		var listItem = Object.Instantiate(prefab);
		listItem.transform.SetParent(_modulesContent, false);
		foreach (var com in listItem.GetComponentsInChildren<Behaviour>())
		{
			com.enabled = true;
		}
		return listItem;
	}

	private Text InstantiateTextItem(string titleKey, string description)
	{
		var textItem = InstantiateListItem(_moduleDescriptionItemPrefab);
		textItem.transform.Find("Title").GetComponent<Text>().text = Localization.Get(titleKey, true);
		textItem.transform.GetChild(2).GetComponent<Text>().text = description;
		textItem.GetComponent<ContentSizeFitterHelper>().Action = Rebuild;
		return textItem.transform.GetChild(2).GetComponent<Text>();
	}

	public void TogglePopup()
	{
		if (_modulesPopup.activeInHierarchy)
		{
			ClosePopup();
		}
		else
		{
			SendTrackerEvents("ModuleList", "ViewModules");
			_modulesPopup.SetActive(true);
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = true;
			LoadIndex();
		}
	}

	private void SendTrackerEvents(string eventKey, string evaluationEventKey, string moduleType = "", string moduleId = "")
	{
		var eventValues = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(moduleType))
		{
			eventValues.Add(TrackerContextKey.SelectedModuleType.ToString(), moduleType);
		}
		if (!string.IsNullOrEmpty(moduleId))
		{
			eventValues.Add(TrackerContextKey.SelectedModule.ToString(), moduleId);
		}

		TrackerEventSender.SendEvent(new TraceEvent(eventKey, TrackerAsset.Verb.Accessed, eventValues, AccessibleTracker.Accessible.Screen));

		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameActivity, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.Event, evaluationEventKey },
			{ TrackerEvaluationKey.GoalOrientation, "Neutral" },
			{ TrackerEvaluationKey.Tool, "ModuleMenu" }
		});
	}

	public void ClosePopup()
	{
		_nextArrow.gameObject.SetActive(false);
		_backArrow.gameObject.SetActive(false);
		_modulesPopup.transform.parent.GetComponent<Image>().enabled = false;
		_modulesPopup.SetActive(false);
	}
}
