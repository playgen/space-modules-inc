using System;
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
using System.Text;
using GameWork.Core.Audio;
using GameWork.Core.Audio.Clip;
using Newtonsoft.Json;
using Random = System.Random;

public class ScenarioController : ICommandAction
{
	#region Config Classes

	public class ScenarioData
	{
		private readonly string[] _scenarioPaths;
		public readonly string Character;
		public int LevelId;

		//public IntegratedAuthoringToolAsset IAT {  get { return _iat; } }
		//private IntegratedAuthoringToolAsset _iat;

		public ScenarioData(int levelId, string[] scenarioPaths, string character)
		{
			LevelId = levelId;
			_scenarioPaths = scenarioPaths;
			Character = character;
		}

		public IntegratedAuthoringToolAsset GetRandomVariation()
		{
			var rng = new Random();
			int index = rng.Next(_scenarioPaths.Length);
			Debug.Log(_scenarioPaths[index]);
			string error;
			var iat = IntegratedAuthoringToolAsset.LoadFromFile(_scenarioPaths[index], out error);
			//var iat = IntegratedAuthoringToolAsset.LoadFromFile("/Scenarios/KnownCheck-Expert-PC1417Z#04.iat");
			return iat;
		}
	}

	public struct RoundConfig
	{
		public Round[] Rounds { get; set; }
	}

	public struct Round
	{
		public Level[] Levels { get; set; }
		public bool PostQuestions { get; set; }
	}

	public struct Level
	{
		public int Id { get; set; }
		public string Prefix { get; set; }
		public string Character { get; set; }
	}

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

	#endregion

	[SerializeField] private string _scenarioFile = "/Scenarios/SpaceModules/SpaceModulesScenarioA.iat";

	private IntegratedAuthoringToolAsset _integratedAuthoringTool;
	private RolePlayCharacterAsset[] _characters;
	private ScenarioData[] _scenarios;
	private Name _currentStateName;
	private DialogueStateActionDTO[] _currentPlayerDialogue;
	private readonly List<Name> _events = new List<Name>();
	private readonly OrderedDictionary _chatHistory = new OrderedDictionary();
	private string[] _allScenarioPaths;
	private Dictionary<string, int> _scores;

	// Round Based
	public int CurrentLevel;
	public int LevelMax;

	public RolePlayCharacterAsset CurrentCharacter;
	private AudioController _audioController;
	private AudioClipModel _audioClip;
	public event Action<LevelObject[]> RefreshSuccessEvent;
	public event Action<DialogueStateActionDTO[]> GetPlayerDialogueSuccessEvent;
	public event Action<OrderedDictionary, float> GetReviewDataSuccessEvent;
	public event Action<ScoreObject> GetScoreDataSuccessEvent;
	public event Action<string> GetCharacterDialogueSuccessEvent;
	public event Action<string, float> GetCharacterStrongestEmotionSuccessEvent;
	public event Action FinalStateEvent;

	#region Initialization

	public ScenarioController(AudioController audioController)
	{
		AssetManager.Instance.Bridge = new AssetManagerBridge();
		_audioController = audioController;
	}

	public void Initialize()
	{
		LoadScenarios();
		//_integratedAuthoringTool = IntegratedAuthoringToolAsset.LoadFromFile(_scenarioFile);
	}

	private void LoadScenarios()
	{
		_allScenarioPaths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Scenarios"), "*.iat");
		var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "levelconfig.json");
		var streamReader = new StreamReader(streamingAssetsPath);
		var obj = JsonConvert.DeserializeObject<RoundConfig>(streamReader.ReadToEnd());

		// TODO: Round selector here 
		var round = obj.Rounds[1].Levels;
		List<ScenarioData> data = new List<ScenarioData>();
		foreach (var level in round)
		{
			data.Add(CreateScenario(level.Id, level.Prefix, level.Character));
		}
		_scenarios = data.ToArray();
		LevelMax = _scenarios.Length;
	}

	private ScenarioData CreateScenario(int level, string prefix, string character)
	{
		var result = _allScenarioPaths.Where(x => x.Contains(prefix)).ToArray();
		return new ScenarioData(level, result, character);
	}

	public void NextLevel()
	{
		CurrentLevel++;
		_scores = new Dictionary<string, int>();
		var scenario = _scenarios.FirstOrDefault(data => data.LevelId.Equals(CurrentLevel));
		if (scenario != null)
		{
			_integratedAuthoringTool = scenario.GetRandomVariation();
			CurrentCharacter = _integratedAuthoringTool.GetCharacterProfile(scenario.Character);
			CurrentCharacter.Initialize();
			_integratedAuthoringTool.BindToRegistry(CurrentCharacter.DynamicPropertiesRegistry);
		}
	}

	#endregion

	#region Level Select

	public void SetCharacter(string name)
	{
		//CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
		//var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)", CurrentCharacter.Perspective);
		//_events.Add(enterEventRpcOne);
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

	#endregion

	#region Character Interaction

	public void GetPlayerDialogueOptions()
	{
		UpdateCurrentState();
		_currentPlayerDialogue =
			_integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.PLAYER, _currentStateName).ToArray();
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
		if (GetCharacterStrongestEmotionSuccessEvent != null)
			GetCharacterStrongestEmotionSuccessEvent(emotionType, CurrentCharacter.Mood);
	}

	public void SetPlayerAction(Guid actionId)
	{
		var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
		UpdateScore(reply);

		var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.GetMeaningName(),
			reply.GetStylesName());

		_events.Add((Name)string.Format("Event(Action-Start,Player,{0},{1})", actionFormat, CurrentCharacter.CharacterName));
		// Wait?
		_events.Add(
			(Name)string.Format("Event(Action-Finished,Player,{0},{1})", actionFormat, CurrentCharacter.CharacterName));
		_events.Add((Name)string.Format("Event(Property-change,self,DialogueState(Player),{0})", reply.NextState));
		_chatHistory.Add(reply.Utterance, "Player");

		// Update EmotionExpression
		GetCharacterResponse();
	}

	public void GetCharacterResponse()
	{
		var action = CurrentCharacter.PerceptionActionLoop(_events);
		_events.Clear();
		//CurrentCharacter.Update(); // Updates mood over time
		if (action == null)
		{
			return;
		}
		var actionKey = action.ActionName.ToString();
		if (actionKey == "Speak")
		{
			var nextState = action.Parameters[1];
			var dialogues = _integratedAuthoringTool.GetDialogueActions(IntegratedAuthoringToolAsset.AGENT, action.Parameters[0]);
			var characterDialogue =
				dialogues.FirstOrDefault(
					dto =>
						string.Equals(dto.GetMeaningName().ToString(), action.Parameters[2].ToString(),
							StringComparison.CurrentCultureIgnoreCase) &&
						string.Equals(dto.GetStylesName().ToString(), action.Parameters[3].ToString(),
							StringComparison.CurrentCultureIgnoreCase));
			PlayDialogueAudio(characterDialogue.FileName);
			var characterDialogueText = characterDialogue.Utterance;
			//_integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), nextState.ToString());
			_events.Add((Name)string.Format("Event(Property-change,self,DialogueState(Player),{0})", nextState));
			_chatHistory.Add(characterDialogueText, "Client");
			//UpdateCurrentState();

			if (GetCharacterDialogueSuccessEvent != null) GetCharacterDialogueSuccessEvent(characterDialogueText);
		}
		CurrentCharacter.ActionFinished(action);
		GetCharacterStrongestEmotion();
	}

	private void PlayDialogueAudio(string audioName)
	{
		var filePath = Path.Combine(Application.streamingAssetsPath, "Scenarios/Audio/F/" + audioName + ".wav");
		Debug.Log(File.Exists(filePath));

		if (_audioClip != null)
		{
			_audioController.Stop(_audioClip);
		}

		_audioClip = new AudioClipModel()
		{
			Name = filePath
		};

		_audioController.Play(_audioClip,
			onComplete: () =>
			{
				if (_currentStateName == Name.BuildName("End"))
				{
					if (FinalStateEvent != null) FinalStateEvent();
				}
			});
	}

	private void UpdateCurrentState()
	{
		CurrentCharacter.PerceptionActionLoop(_events);
		_events.Clear();
		_currentStateName = (Name)CurrentCharacter.GetBeliefValue("DialogueState(Player)");
	}


	#endregion

	public void GetReviewData()
	{
		if (GetReviewDataSuccessEvent != null) GetReviewDataSuccessEvent(_chatHistory, CurrentCharacter.Mood);
		Reset();
	}

	#region Scoring

	public void GetScoreData()
	{
		//var mood = (CurrentCharacter.Mood + 10) / 20;
		//var stars = Mathf.CeilToInt(mood * 3);
		//var scoreObj = new ScoreObject()
		//{
		//	Stars = stars,
		//	Score = Mathf.CeilToInt(mood * 99999),
		//	ScoreFeedbackToken = "FEEDBACK_" + stars,//(mood >= 0.5) ? "Not bad, keep it up!" : "Try a bit harder next time",
		//	MoodImage = (mood >= 0.5),
		//	EmotionCommentToken = "COMMENT_" + ((mood >= 0.5) ? "POSITIVE" : "NEGATIVE"),
		//	Bonus = Mathf.CeilToInt(mood * 999)
		//};


		//if (GetScoreDataSuccessEvent != null) GetScoreDataSuccessEvent(scoreObj);

		//long score = scoreObj.Score;
		//SUGARManager.GameData.Send("score", score);
		//SUGARManager.GameData.Send("plays", 1);
		//SUGARManager.GameData.Send("stars", stars);
		//SUGARManager.GameData.Send("level_" + CurrentCharacter.CharacterName.ToString().ToLower() + "_stars", stars);
	}

	private void UpdateScore(DialogueStateActionDTO reply)
	{
		foreach (var meaning in reply.Meaning)
		{
			HandleKeywords(meaning);
		}

		foreach (var style in reply.Style)
		{
			HandleKeywords(style);
		}
	}

	private void HandleKeywords(string s)
	{
		char[] delimitedChars = {'(', ')'};

		string[] result = s.Split(delimitedChars);

		if (result.Length > 1)
			switch (result[0])
			{
				case "Inquire":
					break;

				case "FAQ":
					break;

				case "Closure":
					break;

				case "Empathy":
					break;

				case "Polite":
					break;
			}
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

	#endregion

	private void Reset()
	{
		Initialize();
		_chatHistory.Clear();
		_events.Clear();
	}
}