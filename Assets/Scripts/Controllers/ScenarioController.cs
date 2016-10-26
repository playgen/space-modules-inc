using System;
using System.Linq;
using AssetManagerPackage;
using GameWork.Commands.Interfaces;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using UnityEngine;

public class ScenarioController : ICommandAction
{
    private IntegratedAuthoringToolAsset _integratedAuthoringTool;
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
        var _characters = _integratedAuthoringTool.GetAllCharacters();
        RefreshSuccessEvent(_characters.ToArray());
    }
}