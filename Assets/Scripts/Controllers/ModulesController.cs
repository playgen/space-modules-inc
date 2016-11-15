using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameWork.Core.Commands.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class ModulesController : ICommandAction
{
    struct ModuleEntry
    {
        public string Name;
        public string Id;
        public string Type;
        public string Manufacturer;
        public Dictionary<string,string> Description;

    }

    private readonly GameObject _modulesPopup;
    private readonly GameObject _popupContent;
    private readonly GameObject _moduleItemPrefab;
    private readonly GameObject _moduleIndexItemPrefab;
    private readonly GameObject _moduleDescriptionItemPrefab;
    private readonly GameObject _modulesContent;

    private readonly Sprite[] _moduleIcons;
    private int _stateLevel;
    private Dictionary<string, ModuleEntry[]> _modulesDict = new Dictionary<string, ModuleEntry[]>();
    private Button _backButton;
    private const string FilePath = "ModulesDatabase/modules";


    public ModulesController()
    {
        _modulesPopup = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup");
        _popupContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View");
        _modulesContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View/Viewport/Content");
        
        _moduleItemPrefab = Resources.Load("Prefabs/ModuleItem") as GameObject;
        _moduleIndexItemPrefab = Resources.Load("Prefabs/ModuleIndexItem") as GameObject;
        _moduleDescriptionItemPrefab = Resources.Load("Prefabs/ModuleDescriptionItem") as GameObject;
        _moduleIcons = Resources.LoadAll<Sprite>("Sprites/Modules/Icons");

        var backButtonObject = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/BackButton");
        _backButton = backButtonObject.GetComponent<Button>();

        LoadJSONModules();
    }

    private void LoadIndex()
    {
        ClearList();
        var moduleTypes = _modulesDict.Keys.ToArray();

        for (var i = 0; i < moduleTypes.Length; i++)
        {
            var moduleType = moduleTypes[i];
            var listItem = InstantiateListItem(_moduleIndexItemPrefab);

            listItem.transform.FindChild("Text").GetComponent<Text>().text = moduleType;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(moduleType));

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

        var modules = _modulesDict[moduleTypeName];

        for (var i = 0; i < modules.Length; i++)
        {
            var module = modules[i];
            var listItem = InstantiateListItem(_moduleItemPrefab);

            listItem.transform.FindChild("Text").GetComponent<Text>().text = module.Name;
            listItem.transform.FindChild("Id").GetComponent<Text>().text = module.Id;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(moduleTypeName));

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
        _modulesContent.GetComponentInChildren<VerticalLayoutGroup>().enabled = true;
        var moduleItem = GameObject.Instantiate(listItem);
        ClearList();
        moduleItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
        moduleItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        foreach (var description in module.Description)
        {
            var desriptionItem = InstantiateListItem(_moduleDescriptionItemPrefab);
            desriptionItem.transform.FindChild("Title").GetComponent<Text>().text = description.Key;
        }

        _backButton.onClick.AddListener(delegate
        {
            _modulesContent.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
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
        var jsonText = Resources.Load(Path.Combine("ModulesDatabase", "modules")) as TextAsset;
        var json = SimpleJSON.JSON.Parse(jsonText.text)[0];
        var moduleslist = new List<ModuleEntry>();
        for (var i = 0; json[i] != null; i++)
        {
            moduleslist.Add(new ModuleEntry()
            {
                Name = json[i]["Name"],
                Id = json[i]["Id"],
                Type = json[i]["Type"],
                Manufacturer = json[i]["Manufacturer"],
                Description = new Dictionary<string, string>()
                {
                    {"Firmware", json[i]["Description"]["firmware"]},
                    {"OSsupport", json[i]["Description"]["OSsupport"]},
                    {"Info", json[i]["Description"]["Info"]}
                }
            });
        }
        _modulesDict = moduleslist.GroupBy(entry => entry.Type).ToDictionary(k=>k.Key, v=>v.ToArray());


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
            _modulesPopup.SetActive(false);
        }
        else
        {
            _modulesContent.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
            _modulesPopup.SetActive(true);
            LoadIndex();
        }
    }
}
