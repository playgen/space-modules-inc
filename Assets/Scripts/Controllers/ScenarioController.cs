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
    public event Action<string> GetCharacterStrongestEmotionSuccessEvent;

    [SerializeField]
    private string _scenarioFile = "/Scenarios/SIDemo.iat";

    private Name _currentStateName;
    private DialogueStateActionDTO[] _currentPlayerDialogue;
    private List<string> _events = new List<string>();

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
        UpdateCurrentState();
        _currentPlayerDialogue = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.PLAYER, _currentStateName).ToArray();
        if (GetPlayerDialogueSuccessEvent != null) GetPlayerDialogueSuccessEvent(_currentPlayerDialogue);

    }

    public void GetCharacterStrongestEmotion()
    {
        var emotion = CurrentCharacter.GetStrongestActiveEmotion().EmotionType;
        if (GetCharacterStrongestEmotionSuccessEvent != null) GetCharacterStrongestEmotionSuccessEvent(emotion);
    }

    public void SetCharacter(string name)
    {
        CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
    }

    public void SetPlayerAction(Guid actionId)
    {
        UpdateCurrentState(); // Might not need this
        var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
        var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.Meaning, reply.Style);

        _events.Add(string.Format("Event(Action-Start, Player,{0},Client)", actionFormat));
        // Wait?
        _events.Add(string.Format("Event(Action-Finished, Player,{0},Client)", actionFormat));
        _events.Add(string.Format("Event(Property-change,Player,DialogueState(Player),{0})", reply.NextState));

        // Update EmotionExpression
        GetCharacterStrongestEmotion();

        _integratedAuthoringTool.SetDialogueState(CurrentCharacter.CharacterName, reply.NextState);
        UpdateCurrentState();


    }

    private void UpdateCurrentState()
    {
        var currentState = _integratedAuthoringTool.GetCurrentDialogueState(CurrentCharacter.CharacterName);
        _currentStateName = Name.BuildName(currentState);
    }
}