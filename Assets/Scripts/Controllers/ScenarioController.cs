using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagerPackage;
using GameWork.Core.Commands.Interfaces;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using WellFormedNames;
using System.IO;
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

	public class RoundConfig
	{
		public Round[] Rounds { get; set; }
	}

	public class Round
	{
		public Level[] Levels { get; set; }
		public bool PostQuestions { get; set; }
	}

	public class Level
	{
		public int Id { get; set; }
		public string Prefix { get; set; }
		public string Character { get; set; }
	}

	public class ScoreObject
	{
		public int Stars;
		public int Score;
		public string ScoreFeedbackToken;
		public bool MoodImage;
		public string EmotionCommentToken;
		public int Bonus;
	}

	public class LevelObject
	{
		public string Name;
		public int Stars;
	}

	public class ChatObject
	{
		public string Utterence;
		public string Agent;
	}

	#endregion

	private IntegratedAuthoringToolAsset _integratedAuthoringTool;
	private ScenarioData[] _scenarios;
	private Name _currentStateName;
	private DialogueStateActionDTO[] _currentPlayerDialogue;
	private readonly List<Name> _events = new List<Name>();
	private readonly List<ChatObject> _chatHistory = new List<ChatObject>();
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
	public event Action<List<ChatObject>, float> GetReviewDataSuccessEvent;
	public event Action<ScoreObject> GetScoreDataSuccessEvent;
	public event Action<string> GetCharacterDialogueSuccessEvent;
	public event Action<string, float> GetCharacterStrongestEmotionSuccessEvent;
	public event Action StopTalkAnimationEvent;
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

		//var round = obj.Rounds[1].Levels;
		var roundNumber = CommandLineUtility.CustomArgs["round"];
		var round = obj.Rounds[Int32.Parse(roundNumber)].Levels;
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


		_chatHistory.Add(new ChatObject() {Utterence = reply.Utterance, Agent = "Player"});

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
			var characterDialogueText = characterDialogue.Utterance;
			//_integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), nextState.ToString());
			_events.Add((Name)string.Format("Event(Property-change,self,DialogueState(Player),{0})", nextState));
			_chatHistory.Add(new ChatObject() { Utterence = characterDialogueText, Agent = "Client" });
			UpdateCurrentState();
			PlayDialogueAudio(characterDialogue.FileName);



			if (GetCharacterDialogueSuccessEvent != null) GetCharacterDialogueSuccessEvent(characterDialogueText);
		}
		CurrentCharacter.ActionFinished(action);
		GetCharacterStrongestEmotion();
	}

	private void PlayDialogueAudio(string audioName)
	{
		var filePath = Path.Combine(Application.streamingAssetsPath, "Scenarios/Audio/F/" + audioName + ".wav");
		if (File.Exists(filePath))
		{
			if (_audioClip != null)
			{
				_audioController.Stop(_audioClip);
			}

			_audioClip = new AudioClipModel()
			{
				Name = filePath
			};

			_audioController.Play(_audioClip,
				onComplete: HandleEndAudio);
		}
		else
		{
			HandleEndAudio();
		}
	}

	
	#endregion

	private void HandleEndAudio()
	{
		Debug.Log("AUDIO END");
		StopTalkAnimationEvent();

		if (_currentStateName == Name.BuildName("End"))
		{
			if (SUGARManager.CurrentUser != null)
			{
				//TraceScore();
			}
			FinalStateEvent();
		}
	}

	private void UpdateCurrentState()
	{
		CurrentCharacter.PerceptionActionLoop(_events);
		_events.Clear();
		_currentStateName = (Name)CurrentCharacter.GetBeliefValue("DialogueState(Player)");
	}

	public void GetReviewData()
	{
		if (GetReviewDataSuccessEvent != null) GetReviewDataSuccessEvent(_chatHistory, CurrentCharacter.Mood);
		Reset();
	}

	#region Scoring

	public void GetScoreData()
	{
		var mood = (CurrentCharacter.Mood + 10) / 20;
		var calcStars = Mathf.CeilToInt(mood * 3);
		var stars = calcStars < 1 ? 1 : calcStars;
		var scoreObj = new ScoreObject()
		{
			Stars = stars,
			Score = Mathf.CeilToInt(mood * 99999),
			ScoreFeedbackToken = "FEEDBACK_" + stars,//(mood >= 0.5) ? "Not bad, keep it up!" : "Try a bit harder next time",
			MoodImage = (mood >= 0.5),
			EmotionCommentToken = "COMMENT_" + ((mood >= 0.5) ? "POSITIVE" : "NEGATIVE"),
			Bonus = Mathf.CeilToInt(mood * 999)
		};


		if (GetScoreDataSuccessEvent != null) GetScoreDataSuccessEvent(scoreObj);

		long score = scoreObj.Score;
		if (SUGARManager.CurrentUser != null)
		{
			SUGARManager.GameData.Send("score", score);
			SUGARManager.GameData.Send("plays", 1);
			SUGARManager.GameData.Send("stars", stars);
			SUGARManager.GameData.Send("level_" + CurrentCharacter.CharacterName.ToString().ToLower() + "_stars", stars);
		}
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
		{
			int value;
			if (_scores.TryGetValue(result[0], out value))
			{
				_scores[result[0]] = value + Int32.Parse(result[1]);
			}
			else
			{
				_scores.Add(result[0], Int32.Parse(result[1]));
			}
		}
	}

	private void TraceScore()
	{
		// The following string contains the key for the google form that will be used to write trace data
		string sheetsKey = "1FAIpQLSebq1WzlCPSfVIzYJDHA3u2cWUwSp1-5KTvaSyM-4ayQn1eWg";

		/// check for scores before referencing key

		// Here the proper string is conclassed to fill and directly post the trace to a google form
		string directSubmitUrl = "https://docs.google.com/forms/d/e/"
			+ sheetsKey
			+ "/formResponse?entry.1676366924="
			+ SUGARManager.CurrentUser.Id
			+ "&entry.858779356="
			+ SUGARManager.GroupId
			+ "&entry.1962055523="
			+ _scores["Closure"]
			+ "&entry.976064318="
			+ _scores["Empathy"]
			+ "&entry.408530093="
			+ _scores["Faq"]
			+ "&entry.2140003828="
			+ _scores["Inquire"]
			+ "&entry.695568148="
			+ _scores["Polite"]
			+ "&submit=Submit"; // This part ensures direct writing instead of first opening the form

		// The actual write to google
		WWW www = new WWW(directSubmitUrl);
	}


	#endregion

	private void Reset()
	{
		Initialize();
		_chatHistory.Clear();
		_events.Clear();
	}
}