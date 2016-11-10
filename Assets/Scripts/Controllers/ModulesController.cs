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
        public ModuleDescription Description;

    }

    struct ModuleDescription
    {
        public string Firmware;
        public string OSsupport;
        public string Info;
    }

    private readonly GameObject _modulesPopup;
    private List<IGrouping<string, ModuleEntry>> _modulesList = new List<IGrouping<string, ModuleEntry>>();
    private GameObject _popupContent;
    private GameObject _moduleItemPrefab;
    private Sprite[] _moduleIcons;
    private const string FilePath = "ModulesDatabase/modules";


    public ModulesController()
    {
        _modulesPopup = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup");
        _popupContent = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/ModulesContainer/ModulesPopup/Scroll View");
        _moduleItemPrefab = Resources.Load("Prefabs/ModuleItem") as GameObject;
        _moduleIcons = Resources.LoadAll<Sprite>("Sprites/Modules/Icons");
        LoadJSONModules();
    }

    public void Initialize()
    {
        LoadIndex();
    }

    private void LoadIndex()
    {
        ClearList();

        for (var i = 0; i < _modulesList.Count; i++)
        {
            var moduleName = _modulesList[i].Key;
            var listItem = GameObject.Instantiate(_moduleItemPrefab);
            listItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
            listItem.transform.FindChild("Text").GetComponent<Text>().text = moduleName;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(moduleName));
            var offset = i * listItem.GetComponent<RectTransform>().rect.height;
            listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
            var modulesArray = _modulesList[i].ToArray();
            listItem.GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadModules(moduleName, modulesArray);
            });
        }
    }

    private void LoadModules(string moduleTypeName, ModuleEntry[] modules)
    {
        ClearList();

        for (var i = 0; i < modules.Length; i++)
        {
            var moduleName = modules[i].Name;
            var listItem = GameObject.Instantiate(_moduleItemPrefab);
            listItem.transform.SetParent(_popupContent.GetComponent<ScrollRect>().content, false);
            listItem.transform.FindChild("Text").GetComponent<Text>().text = moduleName;
            listItem.transform.FindChild("Icon").GetComponent<Image>().sprite = _moduleIcons.FirstOrDefault(sprite => sprite.name.Equals(moduleName));
            var offset = i * listItem.GetComponent<RectTransform>().rect.height;
            listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
        }

    }


    private void ClearList()
    {
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
                Description = new ModuleDescription()
                {
                    Firmware = json[i]["Description"]["firmware"],
                    OSsupport = json[i]["Description"]["OSsupport"],
                    Info = json[i]["Description"]["Info"]
                }
            });
        }
        _modulesList = moduleslist.GroupBy(entry => entry.Type).ToList();
    }

    public void TogglePopup()
    {
        _modulesPopup.SetActive(!_modulesPopup.activeInHierarchy);
        Initialize();
    }
}
