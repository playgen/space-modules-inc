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

using PlayGen.Unity.Utilities.Loading;
using PlayGen.Unity.Utilities.Localization;

using TrackerAssetPackage;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class ScenarioController : ICommandAction
{
	#region Config Classes

	public class ScenarioData
	{
		public readonly string[] ScenarioPaths;
		public readonly string Character;
		public string Prefix;
		public int LevelId;
		public int MaxPoints;

		public ScenarioData(int levelId, string[] scenarioPaths, string character, int maxPoints, string prefix)
		{
			LevelId = levelId;
			Prefix = prefix;
			ScenarioPaths = scenarioPaths;
			Character = character;
			MaxPoints = maxPoints;
		}
	}

	public class RoundConfig
	{
		public Round[] Rounds { get; set; }
	}

	public class Round
	{
		public Level[] Levels { get; set; }
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
		public int Id;
		public int Stars;
	}

	public class ChatObject
	{
		public string Utterence;
		public string Agent;
		public string CurrentState;
		public string NextState;
		public string FileName;
		public string UtteranceId;
		public Guid DialogueId;
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
		EndGame = 0,
		/// <summary>
		/// Feedback at the end of game and broken down in the chat history
		/// </summary>
		InReview,
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
	private string[] _allScenarioPaths;
	private AudioClipModel _audioClip;
	private Random random = new Random();
	private bool _isDemo;

	private readonly List<Name> _events = new List<Name>();
	private readonly List<ChatScoreObject> _chatScoreHistory = new List<ChatScoreObject>();
	private readonly Dictionary<string, int> _feedbackScores = new Dictionary<string, int>();
	private readonly AudioController _audioController;
	private readonly string _demoScenarioPrefix = "TestScen";
	private readonly string _demoUtterance = "";

	// Round Based
	public int CurrentLevel;
	public int LevelMax;
	public bool UseInGameQuestionnaire;
	public RolePlayCharacterAsset CurrentCharacter;
	public bool IsTalking;
	public ScenarioData CurrentScenario;
	public string ScenarioCode;
	public FeedbackMode FeedbackLevel;
	public int RoundNumber;

	public event Action<DialogueStateActionDTO[]> GetPlayerDialogueSuccessEvent;
	public event Action<List<ChatScoreObject>, float, FeedbackMode> GetReviewDataSuccessEvent;
	public event Action<Dictionary<string, int>, FeedbackMode> GetFeedbackEvent;
	public event Action<ScoreObject> GetScoreDataSuccessEvent;
	public event Action<string> GetCharacterDialogueSuccessEvent;
	public event Action<string, float> GetCharacterStrongestEmotionSuccessEvent;
	public event Action StopTalkAnimationEvent;
	public event Action FinalStateEvent;
	public event Action<LevelObject[]> RefreshSuccessEvent;
	public event Action SetLevelSuccessEvent;

	private readonly List<TrackerContextKey> _scoreMetrics = new List<TrackerContextKey>
	{
		TrackerContextKey.Closure,
		TrackerContextKey.Empathy,
		TrackerContextKey.Faq,
		TrackerContextKey.Inquire,
		TrackerContextKey.Polite
	};

	#region Initialization

	public ScenarioController(AudioController audioController)
	{
		AssetManager.Instance.Bridge = new AssetManagerBridge();
		_audioController = audioController;
	}

	public void Initialize()
	{
		_allScenarioPaths = Resources.LoadAll("Scenarios").Select(t => t.name).ToArray();
		var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "levelconfig.json");
		var www = new WWW((Application.platform != RuntimePlatform.Android ? "file:///" : string.Empty) + streamingAssetsPath);
		while (!www.isDone)
		{
		}
		var obj = JsonConvert.DeserializeObject<RoundConfig>(www.text);


		// If no round value is provided in CustomArgs, Round is set to 1 if lockafterq is a CustomArgs key and is true, set to 2 otherwise
		var parseLockAfterQ = false;
		if (CommandLineUtility.CustomArgs.ContainsKey("lockafterq"))
		{
			bool.TryParse(CommandLineUtility.CustomArgs["lockafterq"], out parseLockAfterQ);
		}

		var hasPilotArgs = CommandLineUtility.CustomArgs.ContainsKey("feedback") &&
		                     CommandLineUtility.CustomArgs.ContainsKey("lockafterq");
		var gameUnlocked = PlayerPrefs.GetInt("GameUnlocked") == 1;


		RoundNumber = SUGARManager.CurrentUser != null && hasPilotArgs
			? parseLockAfterQ
				? 1
				: 2
			: gameUnlocked 
				? 2
				: 0;

		
		int parseFeedback;
		FeedbackLevel = SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("feedback") 
			? int.TryParse(CommandLineUtility.CustomArgs["feedback"], out parseFeedback) 
				? (FeedbackMode)parseFeedback 
				: FeedbackMode.EndGame 
			: (FeedbackMode)PlayerPrefs.GetInt("Feedback", (int)FeedbackMode.EndGame);

		PlayerPrefs.SetInt("Feedback", (int)FeedbackLevel);

		var round = obj.Rounds[RoundNumber];
		CurrentLevel = PlayerPrefs.GetInt("CurrentLevel" + RoundNumber, 0);

		_isDemo = (CommandLineUtility.CustomArgs == null || CommandLineUtility.CustomArgs.Count == 0)
		          && !hasPilotArgs
				  && !gameUnlocked
				  && CurrentLevel <= 0;

		if (_isDemo)
		{
			var prefix = _demoScenarioPrefix + _demoUtterance;
			_scenarios = round.Levels.Select(level => new ScenarioData(level.Id, _allScenarioPaths.Where(x => x.Contains(prefix)).ToArray(), level.Character, level.MaxPoints, level.Prefix)).ToArray();
		}
		else
		{
			_scenarios = round.Levels.Select(level => new ScenarioData(level.Id, _allScenarioPaths.Where(x => x.Contains(level.Prefix)).ToArray(), level.Character, level.MaxPoints, level.Prefix)).ToArray();
		}

		LevelMax = _scenarios.Length;

		// Boolean for checking if the post game questionnaire is opened after the round
		bool parseInGameQ;
		UseInGameQuestionnaire = SUGARManager.CurrentUser != null && CommandLineUtility.CustomArgs.ContainsKey("ingameq") && bool.TryParse(CommandLineUtility.CustomArgs["ingameq"], out parseInGameQ) && parseInGameQ;
		if (CurrentLevel >= LevelMax)
		{
			if (SUGARManager.CurrentUser != null)
			{
				if (parseLockAfterQ)
				{
					CurrentLevel = LevelMax;
				}
				else
				{
					PlayerPrefs.SetInt("GameUnlocked", 1);
					CurrentLevel = 0;
				}
			}
			else
			{
				CurrentLevel = 0;
			}
		}

	}

	public void NextLevel()
	{
		CurrentLevel++;

		_feedbackScores.Clear();
		GetFeedbackEvent?.Invoke(_feedbackScores, FeedbackLevel);
		if (_scenarios.Any(data => data.LevelId.Equals(CurrentLevel)))
		{
			CurrentScenario = _scenarios.First(data => data.LevelId.Equals(CurrentLevel));
		}
		else
		{
			var validPaths = _allScenarioPaths.Where(p => p.Contains('#')).ToArray();
			var prefixes = validPaths.Select(p => p.Substring(0, p.IndexOf('-', p.IndexOf('-') + 1) + 1)).ToList();
			var character = new List<string> { "Positive", "Neutral", "Negative" }.OrderBy(dto => random.Next()).First();

			var prefix = _isDemo
				? _demoScenarioPrefix + _demoUtterance
				: prefixes.OrderBy(dto => random.Next()).First();

			// MaxPoints currently hardcoded to 8 for random scenarios.
			// TODO Review validity/balance of MaxPoints = 8
			CurrentScenario = new ScenarioData(CurrentLevel, _allScenarioPaths.Where(x => x.Contains(prefix)).ToArray(), character, 8, prefix);
		}
		if (CurrentScenario != null)
		{
			var index = random.Next(CurrentScenario.ScenarioPaths.Length);
			Debug.Log(CurrentScenario.ScenarioPaths[index]);
			string error;

			ScenarioCode = _isDemo
				? _demoUtterance.Split('#')[0]
				: CurrentScenario.ScenarioPaths[index].Replace(CurrentScenario.Prefix, string.Empty).Split('#')[0];

			_integratedAuthoringTool = IntegratedAuthoringToolAsset.LoadFromFile(Path.Combine("Scenarios", CurrentScenario.ScenarioPaths[index]), out error);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError(error);
			}
			CurrentCharacter = RolePlayCharacterAsset.LoadFromFile(_integratedAuthoringTool.GetAllCharacterSources().First(c => c.Source.Contains(CurrentScenario.Character)).Source);
			CurrentCharacter.LoadAssociatedAssets();
			_integratedAuthoringTool.BindToRegistry(CurrentCharacter.DynamicPropertiesRegistry);
			CurrentCharacter.BodyName = random.NextDouble() >= 0.5 ? "Male" : "Female";
		}
	}

	public void NextQuestionnaire()
	{
		// Pilot logic to set level id to match CurrentLevel divided by 5, as questionnaires occur every 5 levels. Needs changing if occurance rate of questionnaires change 
		CurrentScenario = new ScenarioData(CurrentLevel / 5, _allScenarioPaths.Where(x => x.Contains("Questions")).ToArray(), "Neutral", 0, "Questionnaire");
		string error;
		// Pilot logic to set different questionnaire depending on how many levels have been played. Needs changing if expected appearence of questionnaire changes
		ScenarioCode = (CurrentLevel < LevelMax && LevelMax > 0 ? 1 : 2).ToString();
		_integratedAuthoringTool = IntegratedAuthoringToolAsset.LoadFromFile(Path.Combine("Scenarios", _allScenarioPaths.First(x => x.Contains("Questions" + ScenarioCode))), out error);
		if (!string.IsNullOrEmpty(error))
		{
			Debug.LogError(error);
		}
		CurrentCharacter = RolePlayCharacterAsset.LoadFromFile(_integratedAuthoringTool.GetAllCharacterSources().First(c => c.Source.Contains("Neutral")).Source);
		CurrentCharacter.LoadAssociatedAssets();
		_integratedAuthoringTool.BindToRegistry(CurrentCharacter.DynamicPropertiesRegistry);
	}

	#endregion

	#region Level Select

	// Called after selecting a level
	public void SetLevel(int id)
	{
		CurrentLevel = id - 1;
		SetLevelSuccessEvent?.Invoke();
	}

	// Gets all the characters in the scenario - might need to be changed to get all the scenario variations
	public void RefreshLevelData()
	{
		// TODO: Account for when running a round with no scenarios (currently results in level select with no levels to select)
		Loading.Start();
		var levelList = new LevelObject[_scenarios.Length];
		var sugarKeys = new string[_scenarios.Length];
		for (var i = 0; i < sugarKeys.Length; i++)
		{
			sugarKeys[i] = $"stars_{RoundNumber}_{i + 1}";
		}
		SUGARManager.GameData.Get(responses =>
		{
			var responseList = responses.ToList();
			for (var i = 0; i < _scenarios.Length; i++)
			{
				var key = sugarKeys[i];
				var mostStars = responseList.Where(r => r.Key == key).Select(r => Convert.ToInt16(r.Value)).DefaultIfEmpty().Max();
				levelList[i] = new LevelObject { Id = _scenarios[i].LevelId, Stars = mostStars };
			}
			Loading.Stop();
			RefreshSuccessEvent?.Invoke(levelList);
		}, sugarKeys);
	}
	#endregion

	#region Character Interaction

	public void GetPlayerDialogueOptions()
	{
		UpdateCurrentState();
		_currentPlayerDialogue = _integratedAuthoringTool.GetDialogueActionsByState(_currentStateName.ToString()).ToArray();
		GetPlayerDialogueSuccessEvent?.Invoke(_currentPlayerDialogue);
	}

	public void GetCharacterStrongestEmotion()
	{
		var emotion = CurrentCharacter.GetStrongestActiveEmotion();
		GetCharacterStrongestEmotionSuccessEvent?.Invoke(emotion?.EmotionType, CurrentCharacter.Mood);
	}

	public void SetPlayerAction(Guid actionId)
	{
		var reply = _currentPlayerDialogue.FirstOrDefault(a => a.Id.Equals(actionId));
		if (reply != null && _chatScoreHistory.LastOrDefault(c => c.ChatObject.Agent == "Player")?.ChatObject.CurrentState != reply.CurrentState)
		{
			var actionFormat = $"Speak({reply.CurrentState},{reply.NextState},{reply.Meaning},{reply.Style})";

			// Submit dialogue choice to the IAT event list.
			_events.Add(EventHelper.ActionStart(IATConsts.PLAYER, actionFormat, CurrentCharacter.CharacterName.ToString()));
			_events.Add(EventHelper.ActionEnd(IATConsts.PLAYER, actionFormat, CurrentCharacter.CharacterName.ToString()));
			_events.Add(EventHelper.PropertyChange(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER), reply.NextState, "Player"));

			// UCM tracker tracks the filename ID of each player dialogue choice made
			TrackerEventSender.SendEvent(new TraceEvent("DialogueSelection", TrackerAsset.Verb.Initialized, new Dictionary<string, string>
			{
				{ TrackerContextKey.PlayerDialogueState.ToString(), reply.CurrentState },
				{ TrackerContextKey.PlayerDialogueCode.ToString(), reply.FileName },
				{ TrackerContextKey.PlayerDialogueText.ToString(), reply.Utterance }
			}));
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.AssetActivity, new Dictionary<TrackerEvaluationKey, string>
			{
				{ TrackerEvaluationKey.AssetId, "FAtiMA" },
				{ TrackerEvaluationKey.Action, "DialogueSelection" }
			});
			TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameActivity, new Dictionary<TrackerEvaluationKey, string>
			{
				{ TrackerEvaluationKey.Event, "DialogueSelection" },
				{ TrackerEvaluationKey.GoalOrientation, "Progression" },
				{ TrackerEvaluationKey.Tool, "DialogueChoices" }
			});
			_feedbackScores.Clear();
			UpdateFeedbackScores(reply, "Player");
			GetCharacterResponse();
			GetFeedbackEvent?.Invoke(_feedbackScores, FeedbackLevel);
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
				var characterDialogue = dialogues.FirstOrDefault(dto => string.Equals(dto.Meaning.ToString(), action.Parameters[2].ToString(), StringComparison.CurrentCultureIgnoreCase) && string.Equals(dto.Style.ToString(), action.Parameters[3].ToString(), StringComparison.CurrentCultureIgnoreCase));
				if (characterDialogue != null)
				{
					UpdateFeedbackScores(characterDialogue, "Client");
					_events.Add((Name)string.Format("Event(Property-Change,{0},DialogueState(Player),{1})", CurrentCharacter.CharacterName, nextState));
					UpdateCurrentState();
					PlayDialogueAudio(characterDialogue.FileName);
					GetCharacterDialogueSuccessEvent?.Invoke(Localization.GetAndFormat(characterDialogue.FileName, false, ScenarioCode));
				}
			}
		}

		GetCharacterStrongestEmotion();
	}

	private void UpdateFeedbackScores(DialogueStateActionDTO reply, string agent)
	{
		var chat = new ChatObject
		{
			Utterence = Localization.GetAndFormat(reply.FileName, false, ScenarioCode),
			Agent = agent,
			CurrentState = reply.CurrentState,
			NextState = reply.NextState,
			FileName = reply.FileName,
			UtteranceId = reply.UtteranceId,
			DialogueId = reply.Id
		};
		if (CurrentScenario.Prefix != "Questionnaire")
		{
			_chatScoreHistory.Add(new ChatScoreObject
			{
				ChatObject = chat,
				Scores = new Dictionary<string, int>()
			});

			UpdateScore(reply);
			foreach (var chatScore in _chatScoreHistory.Last().Scores)
			{
				if (_feedbackScores.ContainsKey(chatScore.Key))
				{
					_feedbackScores[chatScore.Key] = chatScore.Value;
				}
				else
				{
					_feedbackScores.Add(chatScore.Key, chatScore.Value);
				}
			}
		}
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
			_audioController.Stop(_audioClip);
		}

		if (Resources.Load<AudioClip>(Path.Combine("Audio", CurrentCharacter.BodyName, audioName)))
		{
			_audioClip = new AudioClipModel
			{
				Name = Path.Combine("Audio", CurrentCharacter.BodyName, audioName)
			};

			if (!string.IsNullOrEmpty(_audioClip.Name))
			{
				IsTalking = true;
				_audioController.Play(_audioClip, onComplete: HandleEndAudio);
				return;
			}
		}
		HandleEndAudio();
	}

	private void HandleEndAudio()
	{
		IsTalking = false;
		StopTalkAnimationEvent?.Invoke();

		if (_currentStateName == Name.BuildName("End"))
		{
			if (SUGARManager.CurrentUser != null && CurrentScenario.Prefix != "Questionnaire")
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
		// Scores required to get certain star ratings semi-hard coded. If balance is off, change these values
		var stars = scoreTotal < CurrentScenario.MaxPoints * 0.4f ? 1 : scoreTotal <= CurrentScenario.MaxPoints * 0.8f ? 2 : 3;

		var allScores = GetScoresByKey();
		var measuredPoints = new Dictionary<string, int>();
		foreach (var scoreMetric in _scoreMetrics)
		{
			measuredPoints.Add(scoreMetric.ToString(), GetScore(allScores, scoreMetric.ToString()));
		}

		var scoreObj = new ScoreObject
		{
			Stars = stars,
			Score = GetPlayerScore(stars, mood),
			ScoreFeedbackToken = "FEEDBACK_" + stars,
			MoodImage = mood >= 0.5,
			EmotionCommentToken = "COMMENT_" + (mood >= 0.5 ? "POSITIVE" : "NEGATIVE"),
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
			SUGARManager.GameData.Send($"stars_{RoundNumber}_{CurrentLevel}", stars);
			foreach (var scoreMetric in _scoreMetrics)
			{
				SUGARManager.GameData.Send(scoreMetric.ToString().ToLower(), GetScore(allScores, scoreMetric.ToString()));
			}
		}
		if (LevelMax > 0)
		{
			PlayerPrefs.SetInt("CurrentLevel" + RoundNumber, CurrentLevel);
		}
		Reset();
	}

	private int GetPlayerScore(int stars, float mood)
	{
		// will set the player score based on score and player mood and normalize between 2 values depending on score
		// 1 star: 100 - 1000
		// 2 stars: 1001 - 5000
		// 3 stars: 5001 - 15000

		int min;
		int max;
		switch (stars)
		{
			case 3:
				min = 5001;
				max = 15000;
				break;
			case 2:
				min = 1001;
				max = 5000;
				break;
			default:
				min = 100;
				max = 1000;
				break;
		}

		var diff = max - min;
		var score = min + Mathf.RoundToInt(diff * mood);
		return score;
	}

	private void UpdateScore(DialogueStateActionDTO reply)
	{
		HandleKeywords(reply.Meaning);
		HandleKeywords(reply.Style);
	}

	// extract scores from Meanings and Styles
	private void HandleKeywords(string s)
	{
		char[] delimitedChars = { '(', ')' };

		var result = s.Split(delimitedChars);

		if (result.Length > 1)
		{
			var chat = _chatScoreHistory.Last().ChatObject;
			var keywords = result[0].Split('_');
			foreach (var keyword in keywords)
			{
				int parseScore;
				int.TryParse(result[1], out parseScore);
				SaveChatScore(chat, keyword, parseScore);
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

		var eventValues = new Dictionary<string, string>
		{
			{TrackerContextKey.LevelNumber.ToString(), CurrentLevel.ToString()},
			{TrackerContextKey.MaxPoints.ToString(), CurrentScenario.MaxPoints.ToString()}
		};
		foreach (var scoreMetric in _scoreMetrics)
		{
			var score = GetScore(allScores, scoreMetric.ToString());
			eventValues.Add(scoreMetric.ToString(), score.ToString());
		}
		TrackerEventSender.SendEvent(new TraceEvent("LevelComplete", TrackerAsset.Verb.Initialized, eventValues));
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
	}

	private void Reset()
	{
		Initialize();
		_chatScoreHistory.Clear();
		_events.Clear();
	}
}