using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using GameWork.Interfacing;
using RolePlayCharacter;
using UnityEngine;

public class GameStateInterface : StateInterface
{

    private GameObject _characterPrefab;
    private GameObject _characterPanel;

    public override void Initialize()
    {
        _characterPrefab = Resources.Load("Prefabs/Characters/Female") as GameObject;
        _characterPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/CharacterPanel");
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
    }

    public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
    {
        var characterObject = GameObject.Instantiate(_characterPrefab);
        characterObject.GetComponent<CharacterFaceController>().CharacterId = currentCharacter.BodyName;
        characterObject.GetComponent<CharacterFaceController>().Gender = currentCharacter.GetBeliefValue("Gender");
        characterObject.transform.SetParent(_characterPanel.transform);

    }

}
