using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AssetManagerPackage;
using GameWork.Core.Commands.Interfaces;
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
    public event Action<string> GetCharacterDialogueSuccessEvent;
    public event Action<string> GetCharacterStrongestEmotionSuccessEvent;

    [SerializeField]
    private string _scenarioFile = "/Scenarios/SpaceModules/SpaceModulesScenarioA.iat";

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
        var emotion = CurrentCharacter.GetStrongestActiveEmotion();
        if (emotion != null)
        {
            var emotionType = emotion.EmotionType;
            if (GetCharacterStrongestEmotionSuccessEvent != null) GetCharacterStrongestEmotionSuccessEvent(emotionType);
        }
    }

    public void SetCharacter(string name)
    {
        CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
        var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)", CurrentCharacter.Perspective);
        _events.Add(enterEventRpcOne);
    }

    public void SetPlayerAction(Guid actionId)
    {
        var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
        var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.Meaning[0], reply.Style[0]);

        _events.Add(string.Format("Event(Action-Start,Player,{0},{1})", actionFormat, CurrentCharacter.Perspective));
        // Wait?
        _events.Add(string.Format("Event(Action-Finished,Player,{0},{1})", actionFormat, CurrentCharacter.Perspective));
        _events.Add(string.Format("Event(Property-change,Player,DialogueState(Player),{0})", reply.NextState));

        _integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), reply.NextState);
        // Update EmotionExpression
        GetCharacterResponse();
        //GetPlayerDialogueOptions();
    }

    public void GetCharacterResponse()
    {
        GetCharacterStrongestEmotion();
        var action = CurrentCharacter.PerceptionActionLoop(_events);
        _events.Clear();
        //CurrentCharacter.Update();
        if (action == null)
        {
            return;
        }
        var actionKey = action.ActionName.ToString();
        if (actionKey == "Speak")
        {
            var nextState = action.Parameters[1];
            var dialogues = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.AGENT, action.Parameters[0]);
            var characterDialogue = dialogues.FirstOrDefault(dto => string.Equals(dto.Meaning[0], action.Parameters[2].ToString(), StringComparison.CurrentCultureIgnoreCase) && string.Equals(dto.Style[0], action.Parameters[3].ToString(), StringComparison.CurrentCultureIgnoreCase));

            var characterDialogueText = characterDialogue.Utterance;
            _integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), nextState.ToString());

            if (GetCharacterDialogueSuccessEvent != null) GetCharacterDialogueSuccessEvent(characterDialogueText);

            UpdateCurrentState();
        }
        CurrentCharacter.ActionFinished(action);

    }

    private void UpdateCurrentState()
    {
        var currentState = _integratedAuthoringTool.GetCurrentDialogueState(CurrentCharacter.CharacterName);
        _currentStateName = Name.BuildName(currentState);
    }
}