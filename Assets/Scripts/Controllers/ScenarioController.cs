﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using AssetManagerPackage;
using GameWork.Core.Commands.Interfaces;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using PlayGen.SUGAR.Common.Shared;
using RolePlayCharacter;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using WellFormedNames;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class ScenarioController : ICommandAction
{
    public struct ScoreObject
    {
        public int Stars;
        public int Score;
        public string ScoreFeedbackToken;
        public bool MoodImage;
        public string EmotionCommentToken;
        public int Bonus;
    }

    public struct LevelObject
    {
        public string Name;
        public int Stars;
    }

    [SerializeField]
    private string _scenarioFile = "/Scenarios/SpaceModules/SpaceModulesScenarioA.iat";

    private IntegratedAuthoringToolAsset _integratedAuthoringTool;
    private RolePlayCharacterAsset[] _characters;
	private ScenarioData[] _scenarios;
    private Name _currentStateName;
    private DialogueStateActionDTO[] _currentPlayerDialogue;
    private readonly List<string> _events = new List<string>();
    private readonly OrderedDictionary _chatHistory = new OrderedDictionary();
	private string[] _allScenarioPaths;

    public RolePlayCharacterAsset CurrentCharacter;
    public event Action<LevelObject[]> RefreshSuccessEvent;
    public event Action<DialogueStateActionDTO[]> GetPlayerDialogueSuccessEvent;
    public event Action<OrderedDictionary, float> GetReviewDataSuccessEvent;
    public event Action<ScoreObject> GetScoreDataSuccessEvent;
    public event Action<string> GetCharacterDialogueSuccessEvent;
    public event Action<string, float> GetCharacterStrongestEmotionSuccessEvent;
    public event Action FinalStateEvent;

    public ScenarioController()
    {
        AssetManager.Instance.Bridge = new AssetManagerBridge();
    }

    public void Initialize()
    {
		Debug.Log("WOOT");
		LoadScenarios();
		//_integratedAuthoringTool = IntegratedAuthoringToolAsset.LoadFromFile(_scenarioFile);
    }

	public class ScenarioData
	{
		public readonly string[] ScenarioPaths;
		//public IntegratedAuthoringToolAsset IAT {  get { return _iat; } }
		//private IntegratedAuthoringToolAsset _iat;

		public ScenarioData(string prefix)
		{
			//LoadAllScenarioVariations(prefix);
			//_iat = IntegratedAuthoringToolAsset.LoadFromFile(Prefix);
		}

	}

	public struct RoundConfig
	{
		 public string[][] Rounds { get; set; }
	}


	private void LoadScenarios()
	{
		_allScenarioPaths = Directory.GetFiles(Application.streamingAssetsPath + "/Scenarios", "*.iat");
		var streamingAssetsPath = Application.streamingAssetsPath + "/levelconfig.json";
		var streamReader = new StreamReader(streamingAssetsPath);
		var obj = JsonConvert.DeserializeObject<RoundConfig>(streamReader.ReadToEnd());
		var roundPaths = obj.Rounds.ToArray();

		// TODO: Round selector here 
		var round = roundPaths[0];
		List<ScenarioData> data = new List<ScenarioData>();
		foreach (var level in round)
		{
			Debug.Log(level);
			CreateScenario(level);
			//data.Add(new ScenarioData(path));
		}
		_scenarios = data.ToArray();
	}

	private ScenarioData CreateScenario(string prefix)
	{
		var result = _allScenarioPaths.Where(x => x.Contains(prefix)).ToArray();
		Debug.Log(result.Length);
		Debug.Log(result[2]);


		return null;
	}

	public void RefreshCharacterArray()
    {
        //_characters = _integratedAuthoringTool.GetAllCharacters().ToArray();
        //var levelList = _characters.ToDictionary(k => "level_" + k.CharacterName.ToLower() + "_stars", v => new LevelObject() {Name = v.CharacterName});

        //var stars = SUGARManager.GameData.GetHighest(_characters.Select(asset => "level_" + asset.CharacterName.ToLower() + "_stars").ToArray(), EvaluationDataType.Long);

        //foreach (var star in stars)
        //{
        //    LevelObject levelObject;
        //    if (levelList.TryGetValue(star.Key, out levelObject))
        //    {
        //        levelObject.Stars = Int32.Parse(star.Value);
        //    }
        //    levelList[star.Key] = levelObject;
        //}

        //if (RefreshSuccessEvent != null) RefreshSuccessEvent(levelList.Values.ToArray());
    }

    public void GetPlayerDialogueOptions()
    {
        //UpdateCurrentState();
        _currentPlayerDialogue = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.PLAYER, _currentStateName).ToArray();
        if (GetPlayerDialogueSuccessEvent != null) GetPlayerDialogueSuccessEvent(_currentPlayerDialogue);
    }

    public void GetCharacterStrongestEmotion()
    {
        var emotion = CurrentCharacter.GetStrongestActiveEmotion();
        string emotionType = null;
        if (emotion != null)
        {
            emotionType = emotion.EmotionType;
        }
        if (GetCharacterStrongestEmotionSuccessEvent != null) GetCharacterStrongestEmotionSuccessEvent(emotionType, CurrentCharacter.Mood);

    }

    public void GetReviewData()
    {
        if (GetReviewDataSuccessEvent != null) GetReviewDataSuccessEvent(_chatHistory, CurrentCharacter.Mood);
        Reset();
    }

    public void GetScoreData()
    {
        //var mood = (CurrentCharacter.Mood + 10) / 20;
        //var stars = Mathf.CeilToInt(mood*3);
        //var scoreObj = new ScoreObject()
        //{
        //    Stars = stars,
        //    Score = Mathf.CeilToInt(mood*99999),
        //    ScoreFeedbackToken = "FEEDBACK_" + stars,//(mood >= 0.5) ? "Not bad, keep it up!" : "Try a bit harder next time",
        //    MoodImage = (mood >= 0.5),
        //    EmotionCommentToken = "COMMENT_" + ((mood >= 0.5) ? "POSITIVE" : "NEGATIVE"),
        //    Bonus = Mathf.CeilToInt(mood*999)
        //};


        //if (GetScoreDataSuccessEvent != null) GetScoreDataSuccessEvent(scoreObj);

        //long score = scoreObj.Score;
        //SUGARManager.GameData.Send("score", score);
        //SUGARManager.GameData.Send("plays", 1);
        //SUGARManager.GameData.Send("stars", stars);
        //SUGARManager.GameData.Send("level_" + CurrentCharacter.CharacterName.ToLower() + "_stars", stars);
    }

    // TODO: Code for tracking individual score categories.
    //public void UpdateScore(DialogueStateActionDTO reply)
    //{
    //    // var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.GetMeaningName(), reply.GetStylesName());
    //    // Debug.Log("Dialogue" + reply.Utterance);
    //    // Debug.Log("Dialogue" + reply.Style[0]);
    //    // Debug.Log("Dialogue" + reply.Meaning[0]);
    //    // Debug.Log(reply.Meaning.Length + reply.Utterance);

    //    foreach (var meaning in reply.Meaning)
    //    {
    //        HandleKeywords(meaning);
    //    }

    //    foreach (var style in reply.Style)
    //    {
    //        HandleKeywords(style);
    //    }
    //}

    //private void HandleKeywords(string s)
    //{
    //    char[] delimitedChars = { '(', ')' };
    //    string[] result = s.Split(delimitedChars);
    //    if (result.Length > 1)
    //        switch (result[0])
    //        {
    //            case "Inquire":
    //                score.GetComponent<ScoreManager>().AddI(Int32.Parse(result[1]));
    //                break;

    //            case "FAQ":
    //                score.GetComponent<ScoreManager>().AddF(Int32.Parse(result[1]));
    //                break;

    //            case "Closure":
    //                score.GetComponent<ScoreManager>().AddC(Int32.Parse(result[1]));
    //                break;

    //            case "Empathy":
    //                score.GetComponent<ScoreManager>().AddE(Int32.Parse(result[1]));
    //                break;

    //            case "Polite":
    //                score.GetComponent<ScoreManager>().AddP(Int32.Parse(result[1]));
    //                break;
    //        }
    //}

    public void SetCharacter(string name)
    {
        //CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
        //var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)", CurrentCharacter.Perspective);
        //_events.Add(enterEventRpcOne);
    }

    public void SetPlayerAction(Guid actionId)
    {
        //var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
        //var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.Meaning[0], reply.Style[0]);

        //_events.Add(string.Format("Event(Action-Start,Player,{0},{1})", actionFormat, CurrentCharacter.Perspective));
        //// Wait?
        //_events.Add(string.Format("Event(Action-Finished,Player,{0},{1})", actionFormat, CurrentCharacter.Perspective));
        //_events.Add(string.Format("Event(Property-change,Player,DialogueState(Player),{0})", reply.NextState));

        //_integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), reply.NextState);
        //_chatHistory.Add(reply.Utterance, "Player");

        //// Update EmotionExpression
        //GetCharacterResponse();
        ////GetPlayerDialogueOptions();
    }

    public void GetCharacterResponse()
    {
        //var action = CurrentCharacter.PerceptionActionLoop(_events);
        //_events.Clear();
        ////CurrentCharacter.Update();
        //if (action == null)
        //{
        //    return;
        //}
        //var actionKey = action.ActionName.ToString();
        //if (actionKey == "Speak")
        //{
        //    var nextState = action.Parameters[1];
        //    var dialogues = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.AGENT, action.Parameters[0]);
        //    var characterDialogue = dialogues.FirstOrDefault(dto => string.Equals(dto.Meaning[0], action.Parameters[2].ToString(), StringComparison.CurrentCultureIgnoreCase) && string.Equals(dto.Style[0], action.Parameters[3].ToString(), StringComparison.CurrentCultureIgnoreCase));

        //    var characterDialogueText = characterDialogue.Utterance;
        //    _integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), nextState.ToString());
        //    _chatHistory.Add(characterDialogueText, "Client");
        //    if (GetCharacterDialogueSuccessEvent != null) GetCharacterDialogueSuccessEvent(characterDialogueText);

        //    UpdateCurrentState();
        //}
        //CurrentCharacter.ActionFinished(action);
        //GetCharacterStrongestEmotion();
    }

    private void UpdateCurrentState()
    {
        //var currentState = _integratedAuthoringTool.GetCurrentDialogueState(CurrentCharacter.CharacterName);
        //_currentStateName = Name.BuildName(currentState);
        //if (currentState == "End")
        //{
        //    if (FinalStateEvent != null) FinalStateEvent();
        //}
    }

    private void Reset()
    {
        Initialize();
        _chatHistory.Clear();
        _events.Clear();
    }

}