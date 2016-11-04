using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagerPackage;
using GameWork.Commands.Interfaces;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using UnityEngine;
using WellFormedNames;

public class ScenarioController : ICommandAction
{
    private IntegratedAuthoringToolAsset _integratedAuthoringTool;
    private RolePlayCharacterAsset[] _characters;

    public RolePlayCharacterAsset CurrentCharacter;
    public event Action<RolePlayCharacterAsset[]> RefreshSuccessEvent;
    public event Action<DialogueStateActionDTO[]> GetPlayerDialogueSuccessEvent;

    [SerializeField]
    private string _scenarioFile = "/Scenarios/SIDemo.iat";

    public ScenarioController()
    {
        AssetManager.Instance.Bridge = new AssetManagerBridge();
    }

    public void Initialize()
    {
        _integratedAuthoringTool = IntegratedAuthoringToolAsset.LoadFromFile(_scenarioFile);

    }


    public void RefreshCharacterArray()
    {
        _characters = _integratedAuthoringTool.GetAllCharacters().ToArray();
        if (RefreshSuccessEvent != null) RefreshSuccessEvent(_characters);
    }


    public void GetPlayerDialogueOptions()
    {
        var currentState = _integratedAuthoringTool.GetCurrentDialogueState(CurrentCharacter.CharacterName);
        var stateName = Name.BuildName(currentState);
        var dialogue = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.PLAYER, stateName).ToArray();
        if (GetPlayerDialogueSuccessEvent != null) GetPlayerDialogueSuccessEvent(dialogue);

    }

    public void SetCharacter(string name)
    {
        CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
    }

}