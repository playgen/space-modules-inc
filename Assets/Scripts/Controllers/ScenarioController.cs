﻿using System;
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

using TrackerAssetPackage;

using Random = System.Random;

public class ScenarioController : ICommandAction
{
	#region Config Classes

	public class ScenarioData
	{
		private readonly string[] _scenarioPaths;
		public readonly string Character;
		public string Prefix;
		public int LevelId;
		public int MaxPoints;

		public ScenarioData(int levelId, string[] scenarioPaths, string character, int maxPoints, string prefix)
		{
			LevelId = levelId;
			Prefix = prefix;
			_scenarioPaths = scenarioPaths;
			Character = character;
			MaxPoints = maxPoints;
		}


		public IntegratedAuthoringToolAsset GetRandomVariation()
		{
			var rng = new Random();
			var index = rng.Next(_scenarioPaths.Length);
			Debug.Log(_scenarioPaths[index]);
			string error;
			var iat = IntegratedAuthoringToolAsset.LoadFromFile(Path.Combine("Scenarios", _scenarioPaths[index]), out error);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError(error);
			}
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
		public int MaxPoints { get; set; }
	}

	public class ScoreObject
	{
		public int Stars;
		public int Score;
		public string ScoreFeedbackToken;
		public bool MoodImage;
		public string EmotionCommentToken;
		public int Bonus;
		public Dictionary<string, int> MeasuredPoints;
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
		public string Code;
	}

	public class ChatScoreObject
	{
		public ChatObject ChatObject;
		public Dictionary<string, int> Scores;
	}

	/// <summary>
	/// The feedback mode identifies the level of feedback to give to the player
	/// </summary>
	public enum FeedbackMode
	{
		/// <summary>
		/// Feedback only at the end of game
		/// </summary>
		Minimal = 0, 
		/// <summary>
		/// Feedback at the end of game and broken down in the chat history
		/// </summary>
		EndGame, 
		/// <summary>
		/// Feedback at the end of game, in chat history and during the game
		/// </summary>
		InGame
	}

	#endregion

	private IntegratedAuthoringToolAsset _integratedAuthoringTool;
	private ScenarioData[] _scenarios;
	private Name _currentStateName;
	private DialogueStateActionDTO[] _currentPlayerDialogue;
	private readonly List<Name> _events = new List<Name>();
	private readonly List<ChatScoreObject> _chatScoreHistory = new List<ChatScoreObject>();
	private string[] _allScenarioPaths;
	private Dictionary<string, int> _feedbackScores = new Dictionary<string, int>();

	// Round Based
	public int CurrentLevel;
	public int LevelMax;
	public bool PostQuestions;
	public bool UseInGameQuestionnaire;
	public RolePlayCharacterAsset CurrentCharacter;
	public bool IsTalking;
	public ScenarioData CurrentScenario;
	public FeedbackMode FeedbackLevel;

	private readonly AudioController _audioController;
	private AudioClipModel _audioClip;
	private int _roundNumber;

	public event Action<LevelObject[]> RefreshSuccessEvent;
	public event Action<DialogueStateActionDTO[]> GetPlayerDialogueSuccessEvent;
	public event Action<List<ChatScoreObject>, float, FeedbackMode> GetReviewDataSuccessEvent;
	public event Action<Dictionary<string, int>, FeedbackMode> GetFeedbackEvent;
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
		_allScenarioPaths = Application.platform == RuntimePlatform.WindowsPlayer ? Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Scenarios"), "*.iat") : Resources.LoadAll("Scenarios").Select(t => t.name).ToArray();
		var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "levelconfig.json");
		var www = new WWW((Application.platform != RuntimePlatform.Android ? "file:///" : string.Empty) + streamingAssetsPath);
		while (!www.isDone)
		{
		}
		var obj = JsonConvert.DeserializeObject<RoundConfig>(www.text);

		//var round = obj.Rounds[1].Levels;
		// Takes round number from command line args (minus 1 for SPL not able to pass round=0 via URL)
		if (CommandLineUtility.CustomArgs.ContainsKey("round"))
		{
			_roundNumber = int.Parse(CommandLineUtility.CustomArgs["round"]) - 1;
		}
		else
		{
			_roundNumber = 1;
		}
		var round = obj.Rounds[_roundNumber];
		var data = new List<ScenarioData>();
		foreach (var level in round.Levels)
		{
			data.Add(CreateScenario(level.Id, level.Prefix, level.Character, level.MaxPoints));
		}
		_scenarios = data.ToArray();
		LevelMax = _scenarios.Length;
		// Boolean for checking if the post game questionnaire is opened after the round
		if (CommandLineUtility.CustomArgs.ContainsKey("feedback"))
		{
			var feedback = int.Parse(CommandLineUtility.CustomArgs["feedback"]);
			FeedbackLevel = (FeedbackMode) feedback;
		}
		else
		{
			FeedbackLevel = FeedbackMode.Minimal;
		}

		if (CommandLineUtility.CustomArgs.ContainsKey("ingameq"))
		{
			var useq = bool.Parse(CommandLineUtility.CustomArgs["ingameq"]);
			UseInGameQuestionnaire = useq;
		}
		PostQuestions = round.PostQuestions;
	}

	private ScenarioData CreateScenario(int level, string prefix, string character, int maxPoints)
	{
		var result = _allScenarioPaths.Where(x => x.Contains(prefix)).ToArray();
		return new ScenarioData(level, result, character, maxPoints, prefix);
	}

	public void NextLevel()
	{
		CurrentLevel++;

		_feedbackScores.Clear();
		GetFeedbackEvent?.Invoke(_feedbackScores, FeedbackLevel);
		CurrentScenario = _scenarios.FirstOrDefault(data => data.LevelId.Equals(CurrentLevel));
		if (CurrentScenario != null)
		{
			_integratedAuthoringTool = CurrentScenario.GetRandomVariation();
			CurrentCharacter = RolePlayCharacterAsset.LoadFromFile(_integratedAuthoringTool.GetAllCharacterSources().First(c => c.Source.Contains(CurrentScenario.Character)).Source);
			CurrentCharacter.LoadAssociatedAssets();
			_integratedAuthoringTool.BindToRegistry(CurrentCharacter.DynamicPropertiesRegistry);
			if (CurrentScenario.Prefix == "TestScen")
			{
				CurrentCharacter.BodyName = "Male";
			}
			else
			{
				var rand = new Random();
				CurrentCharacter.BodyName = (rand.NextDouble() >= 0.5) ? "Male" : "Female";
			}
		}
	}

	#endregion

	#region Level Select

	// Called after selecting a level
	public void SetCharacter(string name)
	{
		//CurrentCharacter = _characters.FirstOrDefault(asset => asset.CharacterName.Equals(name));
		//var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)", CurrentCharacter.Perspective);
		//_events.Add(enterEventRpcOne);
	}

	// Gets all the characters in the scenario - might need to be changed to get all the scenario variations
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
			_integratedAuthoringTool.GetDialogueActionsByState(_currentStateName.ToString()).ToArray();
		GetPlayerDialogueSuccessEvent?.Invoke(_currentPlayerDialogue);
	}

	public void GetCharacterStrongestEmotion()
	{
		var emotion = CurrentCharacter.GetStrongestActiveEmotion();
		string emotionType = null;
		if (emotion != null)
		{
			emotionType = emotion.EmotionType;
		}
		GetCharacterStrongestEmotionSuccessEvent?.Invoke(emotionType, CurrentCharacter.Mood);
	}

	public void SetPlayerAction(Guid actionId)
	{
		var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
		if (reply != null)
		{
			var actionFormat = string.Format("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.Meaning, reply.Style);

			// Submit dialogue choice to the IAT event list.
			_events.Add(EventHelper.ActionStart(IATConsts.PLAYER, actionFormat, CurrentCharacter.CharacterName.ToString()));
			_events.Add(EventHelper.ActionEnd(IATConsts.PLAYER, actionFormat, CurrentCharacter.CharacterName.ToString()));
			_events.Add(EventHelper.PropertyChange(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER), reply.NextState, "Player"));

			// UCM tracker tracks the filename ID of each player dialogue choice made
			TrackerEventSender.SendEvent(new TraceEvent("DialogueSelection", TrackerAsset.Verb.Initialized, new Dictionary<string, string>
			{
				{ TrackerContextKeys.PlayerDialogueChoice.ToString(), reply.CurrentState + " " + reply.FileName }
			}));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.AssetActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Asset, "FAtiMA" },
				{ TrackerEvaluationKeys.Done, "true" }
			});
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string>
			{
				{ TrackerEvaluationKeys.Event, "DialogueSelection" },
				{ TrackerEvaluationKeys.GoalOrientation, "Progression" },
				{ TrackerEvaluationKeys.Tool, "DialogueChoices" }
			});

			//Tracker.T.RequestFlush();

			var chat = new ChatObject 
			{
				Utterence = reply.Utterance,
				Agent = "Player",
				Code = reply.CurrentState + "." + reply.FileName
			};
			_chatScoreHistory.Add(new ChatScoreObject
			{
				ChatObject = chat,
				Scores = new Dictionary<string, int>()
			});
            UpdateScore(reply);
            _feedbackScores = _chatScoreHistory.Last().Scores;
            GetFeedbackEvent?.Invoke(_feedbackScores, FeedbackLevel);
            // Update EmotionExpression
            GetCharacterResponse();
        }
	}

	public void GetCharacterResponse()
	{
		CurrentCharacter.Perceive(_events);
		var possibleActions = CurrentCharacter.Decide();
		var action = RolePlayCharacterAsset.TakeBestActions(possibleActions).FirstOrDefault();
		_events.Clear();
		CurrentCharacter.Update();

		if (action != null)
		{
			var actionKey = action.Key.ToString();
			if (actionKey == "Speak")
			{
				var nextState = action.Parameters[1];
				var dialogues = _integratedAuthoringTool.GetDialogueActionsByState(action.Parameters[0].ToString());
				var characterDialogue =
					dialogues.FirstOrDefault(
						dto =>
							string.Equals(dto.Meaning.ToString(), action.Parameters[2].ToString(),
								StringComparison.CurrentCultureIgnoreCase) &&
							string.Equals(dto.Style.ToString(), action.Parameters[3].ToString(),
								StringComparison.CurrentCultureIgnoreCase));
				if (characterDialogue != null)
				{
					var characterDialogueText = characterDialogue.Utterance;

					var chat = new ChatObject
					{
						Utterence = characterDialogueText,
						Agent = "Client",
						Code = characterDialogue.CurrentState + "." + characterDialogue.FileName
					};
					_chatScoreHistory.Add(new ChatScoreObject
					{
						ChatObject = chat,
						Scores = new Dictionary<string, int>()
					});
					UpdateScore(characterDialogue);

					//_integratedAuthoringTool.SetDialogueState(CurrentCharacter.Perspective.ToString(), nextState.ToString());
					_events.Add((Name) string.Format("Event(Property-Change,{0},DialogueState(Player),{1})", CurrentCharacter.CharacterName, nextState));
					UpdateCurrentState();
					PlayDialogueAudio(characterDialogue.FileName);

					GetCharacterDialogueSuccessEvent?.Invoke(characterDialogueText);
				}
			}
		}
		
		GetCharacterStrongestEmotion();
	}

	private void UpdateCurrentState()
	{
		CurrentCharacter.Perceive(_events);
		_events.Clear();
		_currentStateName = (Name)CurrentCharacter.GetBeliefValue("DialogueState(Player)");
	}

	private void PlayDialogueAudio(string audioName)
	{
		if (!string.IsNullOrEmpty(_audioClip?.Name))
		{
			if (_audioClip != null)
			{
				_audioController.Stop(_audioClip);
			}
		}

		_audioClip = new AudioClipModel
		{
			Name = Path.Combine("Audio", CurrentCharacter.BodyName, audioName)
		};

		if (!string.IsNullOrEmpty(_audioClip.Name))
		{
			IsTalking = true;

			_audioController.Play(_audioClip, onComplete: HandleEndAudio);
		}
		else
		{
			HandleEndAudio();
		}
	}

	private void HandleEndAudio()
	{
		IsTalking = false;
		StopTalkAnimationEvent?.Invoke();

		if (_currentStateName == Name.BuildName("End"))
		{
			if (SUGARManager.CurrentUser != null)
			{
				TraceScore();
			}
			FinalStateEvent?.Invoke();
		}
	}

	#endregion

	#region Scoring

	// Score shown to the player (not the tracker)
	public void GetScoreData()
	{
		var mood = (CurrentCharacter.Mood + 10) / 20;
		var scoreTotal = 0;
		foreach (var chatScoreObject in _chatScoreHistory)
		{
			foreach (var i in chatScoreObject.Scores.Values)
			{
				scoreTotal += i;
			}
		}
		var lowerScoreBracket = CurrentScenario.MaxPoints*0.4f;
		var upperScoreBracket = CurrentScenario.MaxPoints*0.8f;
		int stars;
		if (scoreTotal < lowerScoreBracket)
		{
			stars = 1;
		}
		else if (scoreTotal <= upperScoreBracket)
		{
			stars = 2;
		}
		else
		{
			stars = 3;
		}

		// Minimum score
		if (scoreTotal < 1)
		{
			scoreTotal = 1;
		}

		var allScores = GetScoresByKey();

		var measuredPoints = new Dictionary<string, int>
		{
			{"Closure", GetScore(allScores, "Closure")},
			{"Empathy", GetScore(allScores, "Empathy")},
			{"Faq", GetScore(allScores, "Faq")},
			{"Inquire", GetScore(allScores, "Inquire")},
			{"Polite", GetScore(allScores, "Polite")}
		};

		var scoreObj = new ScoreObject
		{
			Stars = stars,
			Score = (int)Math.Pow(10, 2 + (4 * (scoreTotal - 1)/(CurrentScenario.MaxPoints - 1))),
			ScoreFeedbackToken = "FEEDBACK_" + stars,
			MoodImage = (mood >= 0.5),
			EmotionCommentToken = "COMMENT_" + ((mood >= 0.5) ? "POSITIVE" : "NEGATIVE"),
			Bonus = Mathf.CeilToInt(mood * 999),
			MeasuredPoints = measuredPoints
		};

		GetScoreDataSuccessEvent?.Invoke(scoreObj);

		var score = (long)scoreObj.Score;
		// Player score traced by SUGAR
		if (SUGARManager.CurrentUser != null)
		{
			SUGARManager.GameData.Send("score", score);
			SUGARManager.GameData.Send("plays", 1);
			SUGARManager.GameData.Send("stars", stars);
			SUGARManager.GameData.Send("level_" + CurrentCharacter.CharacterName.ToString().ToLower() + "_stars", stars);
		}
		Reset();
	}

	private void UpdateScore(DialogueStateActionDTO reply)
	{
		HandleKeywords(reply.Meaning);
		HandleKeywords(reply.Style);
	}

	// extract scores from Meanings and Styles
	private void HandleKeywords(string s)
	{
		char[] delimitedChars = {'(', ')'};

		var result = s.Split(delimitedChars);

		if (result.Length > 1)
		{
			var chat = _chatScoreHistory.Last().ChatObject;
			var keywords = result[0].Split('_');
			foreach (var keyword in keywords)
			{
				var score = int.Parse(result[1]);
				SaveChatScore(chat, keyword, score);
			}
		}
	}

	private void SaveChatScore(ChatObject chat, string keyword, int score)
	{
		// Check if the list already has an element for the current chat object
		var chatScoreObject = _chatScoreHistory.Find(c => c.ChatObject == chat);

		if (chatScoreObject != null)
		{
			chatScoreObject.Scores.Add(keyword, score);
		}
		else
		{
			
			var newChatScoreObject = new ChatScoreObject
				{
					ChatObject = chat,
					Scores = new Dictionary<string, int>()
				};
			newChatScoreObject.Scores.Add(keyword, score);
			_chatScoreHistory.Add(newChatScoreObject);
		}
	}

	private void TraceScore()
	{
		var allScores = GetScoresByKey();

		// check for scores before referencing key
		var closure = GetScore(allScores, "Closure");
		var empathy = GetScore(allScores, "Empathy");
		var faq = GetScore(allScores, "Faq");
		var inquire = GetScore(allScores, "Inquire");
		var polite = GetScore(allScores, "Polite");

		TrackerEventSender.SendEvent(new TraceEvent("LevelComplete", TrackerAsset.Verb.Initialized, new Dictionary<string, string>
		{
			{ TrackerContextKeys.LevelNumber.ToString(), CurrentLevel.ToString() },
			{ TrackerContextKeys.Closure.ToString(), closure.ToString() },
			{ TrackerContextKeys.Empathy.ToString(), empathy.ToString() },
			{ TrackerContextKeys.Faq.ToString(), faq.ToString() },
			{ TrackerContextKeys.Inquire.ToString(), inquire.ToString() },
			{ TrackerContextKeys.Polite.ToString(), polite.ToString() },
			{ TrackerContextKeys.MaxPoints.ToString(), CurrentScenario.MaxPoints.ToString() }
		}));
	}

	private Dictionary<string, int> GetScoresByKey()
	{
		var scores = new Dictionary<string, int>();

		var all = _chatScoreHistory.FindAll(c => c.Scores.Count > 0);
		foreach (var chatScoreObject in all)
		{
			foreach (var score in chatScoreObject.Scores)
			{
				if (scores.ContainsKey(score.Key))
				{
					scores[score.Key] = scores[score.Key] + score.Value;
				}
				else
				{
					scores[score.Key] = score.Value;
				}
			}
		}

		return scores;
	}

	private int GetScore(Dictionary<string, int> scores, string key)
	{
		return scores.ContainsKey(key) ? scores[key] : 0;
	}

	#endregion

	public void GetReviewData()
	{
		GetReviewDataSuccessEvent?.Invoke(_chatScoreHistory, CurrentCharacter.Mood, FeedbackLevel);

		//Reset();
	}

	private void Reset()
	{
		Initialize();
		_chatScoreHistory.Clear();
		_events.Clear();
	}
}