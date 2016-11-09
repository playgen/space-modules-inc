using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

public class LevelStateInterface : StateInterface
{
    private GameObject _gridLayout;
    private GameObject _itemPrefab;

    public override void Initialize()
    {
        
        _itemPrefab = Resources.Load("Prefabs/LevelItem") as GameObject;

        //GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer/LevelPanel/LevelItem").GetComponent<Button>().onClick.AddListener(LoadLevel);
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer/BackButton").GetComponent<Button>().onClick.AddListener(OnBackClick);

    }

    public override void Enter()
    {
        _gridLayout = GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer/LevelPanel/GridLayout");
        ConfigureGridSize(3, 3);
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer").SetActive(true);
        EnqueueCommand(new RefreshLevelDataCommand());
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
    }

    private void OnBackClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    private void LoadLevel(string name)
    {
        EnqueueCommand(new SetLevelCommand(name));
        EnqueueCommand(new NextStateCommand());
    }

    public void UpdateLevelList(RolePlayCharacterAsset[] Levels)
    {
        // Clear List
        foreach (var cell in _gridLayout.transform)
        {
            var cellGameObject = cell as Transform;
            if (cellGameObject != null) GameObject.Destroy(cellGameObject.gameObject);
        }


        for (var i = 0; i < Levels.Length; i++)
        {
            var levelItem = GameObject.Instantiate(_itemPrefab);
            levelItem.GetComponent<LevelItemBehaviour>().SetupItem(0, "LINE " + i);
            levelItem.transform.SetParent(_gridLayout.transform, false);
            var index = i;
            levelItem.GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadLevel( Levels[index].CharacterName); 
                
            });
        }
    }

    private void ConfigureGridSize(int rows, int cols)
    {
        var gridRect = _gridLayout.GetComponent<RectTransform>();
        var buttonHeight = gridRect.rect.height / rows;           
        var buttonWidth = gridRect.rect.width / cols;            
        var gridLayoutGroup = _gridLayout.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(buttonWidth, buttonHeight);

    }
}
 