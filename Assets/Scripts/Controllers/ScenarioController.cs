using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagerPackage;
using GameWork.Commands.Interfaces;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using UnityEngine;

public class ScenarioController : ICommandAction
{
    private IntegratedAuthoringToolAsset _integratedAuthoringTool;
    private RolePlayCharacterAsset[] _characters;

    public RolePlayCharacterAsset CurrentCharacter;
    public event Action<RolePlayCharacterAsset[]> RefreshSuccessEvent;

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

    public void SetCharacter(string name)
    {
        CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
    }

}