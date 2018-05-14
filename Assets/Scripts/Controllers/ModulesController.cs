﻿using System.Collections.Generic;
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
			TrackerEventSender.SendEvent(new TraceEvent("ClosedModuleMenu", TrackerAsset.Verb.Accessed, new Dictionary<string, string>(), AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ClosedModuleMenu" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
		});

		var www = new WWW((Application.platform != RuntimePlatform.Android ? "file:///" : string.Empty) + Path.Combine(Application.streamingAssetsPath, "modules.json"));
		while (!www.isDone)
		{
		}
		_modulesDatabase = JsonConvert.DeserializeObject<ModuleEntry[]>(www.text);
	}

	private void LoadIndex()
	{
		ClearList();
		var moduleTypes = _modulesDatabase.Select(entry => entry.Type).Distinct().ToArray();

		foreach (var moduleType in moduleTypes)
		{
			var listItem = InstantiateListItem(_moduleIndexItemPrefab);
			var iconId = _modulesDatabase.FirstOrDefault(entry => entry.Type.Equals(moduleType)).Icon;
			listItem.transform.Find("Text").GetComponent<Text>().text = moduleType;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(iconId));
			listItem.GetComponent<Button>().onClick.AddListener(() =>
			{
				LoadModules(moduleType);
				TrackerEventSender.SendEvent(new TraceEvent("SelectedModuleType", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
				{
					{ TrackerContextKeys.SelectedModuleType.ToString(), moduleType }
				}, AccessibleTracker.Accessible.Screen));
				TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
				{
					{ TrackerEvaluationKeys.Event, "SelectedModuleType" },
					{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
					{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
				});
			});
		}

		_backButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			TogglePopup();
			TrackerEventSender.SendEvent(new TraceEvent("CloseModuleMenu", TrackerAsset.Verb.Accessed, new Dictionary<string, string>(), AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "CloseModuleMenu" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
		});
	}

	private void LoadModules(string moduleTypeName)
	{
		ClearList();
		_nextArrow.gameObject.SetActive(false);
		_backArrow.gameObject.SetActive(false);
		var modules = _modulesDatabase.Where(entry => entry.Type.Equals(moduleTypeName)).ToArray();

		foreach (var module in modules)
		{
			var listItem = InstantiateListItem(_moduleItemPrefab);

			listItem.transform.Find("Text").GetComponent<Text>().text = module.Name;
			listItem.transform.Find("Id").GetComponent<Text>().text = module.Id;
			listItem.transform.Find("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(module.Icon));
			listItem.GetComponent<Button>().onClick.AddListener(() =>
			{
				LoadModule(module, listItem);
				TrackerEventSender.SendEvent(new TraceEvent("SelectedModule", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
				{
					{ TrackerContextKeys.SelectedModuleType.ToString(), moduleTypeName },
					{ TrackerContextKeys.SelectedModule.ToString(), module.Id }
				}, AccessibleTracker.Accessible.Screen));
				TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
				{
					{ TrackerEvaluationKeys.Event, "SelectedModule" },
					{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
					{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
				});
			});
		}
		_backButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			LoadIndex();
			TrackerEventSender.SendEvent(new TraceEvent("BackToModuleTypeList", TrackerAsset.Verb.Accessed, new Dictionary<string, string>(), AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "BackToModuleTypeList" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
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
			TrackerEventSender.SendEvent(new TraceEvent("NextModuleFAQ", TrackerAsset.Verb.Accessed, new Dictionary<string, string>
			{
				{ TrackerContextKeys.SelectedModuleType.ToString(), module.Type },
				{ TrackerContextKeys.SelectedModule.ToString(), module.Id }
			}, AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ViewNextProblem" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
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
				{ TrackerContextKeys.SelectedModuleType.ToString(), module.Type },
				{ TrackerContextKeys.SelectedModule.ToString(), module.Id }
			}, AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ViewPreviousProblem" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
		});

		_backButton.onClick.AddListener(() =>
		{
			LoadModules(module.Type);
			TrackerEventSender.SendEvent(new TraceEvent("BackToModuleDeviceTypes", TrackerAsset.Verb.Accessed, new Dictionary<string, string>(), AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "BackToModuleList" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
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
			_nextArrow.gameObject.SetActive(false);
			_backArrow.gameObject.SetActive(false);
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = false;
			_modulesPopup.SetActive(false);

		}
		else
		{
			TrackerEventSender.SendEvent(new TraceEvent("ModuleList", TrackerAsset.Verb.Accessed, new Dictionary<string, string>(), AccessibleTracker.Accessible.Screen));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "ViewModules" },
				{ TrackerEvaluationKeys.GoalOrientation, "Neutral" },
				{ TrackerEvaluationKeys.Tool, "ModuleMenu" }
			});
			_modulesPopup.SetActive(true);
			_modulesPopup.transform.parent.GetComponent<Image>().enabled = true;
			LoadIndex();
		}
	}
}
