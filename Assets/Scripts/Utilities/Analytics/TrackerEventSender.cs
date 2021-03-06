﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PlayGen.SUGAR.Unity;
using RAGE.EvaluationAsset;
using TrackerAssetPackage;
using UnityEngine;

using Tracker = RAGE.Analytics.Tracker;

public class TraceEvent
{
	public string Key;
	public TrackerAsset.Verb ActionType;
	public object[] Params;
	public Dictionary<string, string> Values;

	public TraceEvent(string key, TrackerAsset.Verb verb, Dictionary<string, string> values, params object[] param)
	{
		Key = key;
		ActionType = verb;
		Values = values;
		Params = param;
	}
}

/// <summary>
/// Class used to handle events before passing them to the RAGE tracker
/// </summary>
public class TrackerEventSender
{
	private static ScenarioController _scenarioController;
	private static ModulesController.ModuleEntry[] _moduleDatabase;

	public static void SetScenarioController(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	public static void SetModuleDatabase(ModulesController.ModuleEntry[] moduleDatabase)
	{
		_moduleDatabase = moduleDatabase;
	}

	public static void SendEvent(TraceEvent trace)
	{
		try
		{
			if (SUGARManager.CurrentUser == null)
			{
				Debug.LogWarning("Cannot send events if not currently logged in");
				return;
			}
			foreach (var v in trace.Values.OrderBy(v => v.Key))
			{
				Tracker.T.setVar(v.Key, v.Value);
			}

			Tracker.T.setVar(TrackerContextKey.UserId.ToString(), SUGARManager.CurrentUser.Name);
			Tracker.T.setVar(TrackerContextKey.Round.ToString(), _scenarioController.RoundNumber);
			Tracker.T.setVar(TrackerContextKey.CurrentLevel.ToString(), _scenarioController.CurrentLevel);

			if (!string.IsNullOrEmpty(SUGARManager.ClassId))
			{
				Tracker.T.setVar(TrackerContextKey.GroupId.ToString(), SUGARManager.ClassId);
			}
			Tracker.T.setVar(TrackerContextKey.CurrentScenario.ToString(), _scenarioController.CurrentScenario.Prefix);
			Tracker.T.setVar(TrackerContextKey.CurrentModule.ToString(), _scenarioController.ScenarioCode);
			if (_scenarioController.CurrentScenario.Prefix != "Questionnaire")
			{
				Tracker.T.setVar(TrackerContextKey.CurrentModuleType.ToString(), _moduleDatabase.First(m => m.Id == _scenarioController.ScenarioCode).Type);
				Tracker.T.setVar(TrackerContextKey.CurrentCharacter.ToString(), _scenarioController.CurrentCharacter.VoiceName);
				Tracker.T.setVar(TrackerContextKey.FeedbackMode.ToString(), _scenarioController.FeedbackLevel.ToString());
			}
			switch (trace.ActionType)
			{
				case TrackerAsset.Verb.Accessed:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(AccessibleTracker.Accessible))
					{
						Tracker.T.Accessible.Accessed(trace.Key, (AccessibleTracker.Accessible)Enum.Parse(typeof(AccessibleTracker.Accessible), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.Accessible.Accessed(trace.Key);
					break;
				case TrackerAsset.Verb.Skipped:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(AccessibleTracker.Accessible))
					{
						Tracker.T.Accessible.Skipped(trace.Key, (AccessibleTracker.Accessible)Enum.Parse(typeof(AccessibleTracker.Accessible), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.Accessible.Skipped(trace.Key);
					break;
				case TrackerAsset.Verb.Selected:
					if (trace.Params.Length > 1 && trace.Params[1].GetType() == typeof(AlternativeTracker.Alternative) && trace.Params[0] is string)
					{
						Tracker.T.Alternative.Selected(trace.Key, trace.Params[0].ToString(), (AlternativeTracker.Alternative)Enum.Parse(typeof(AlternativeTracker.Alternative), trace.Params[1].ToString()));
						break;
					}
					if (trace.Params.Length > 0 && trace.Params[0] is string)
					{
						Tracker.T.Alternative.Selected(trace.Key, trace.Params[0].ToString());
						break;
					}
					Tracker.T.GameObject.Interacted(trace.Key);
					break;
				case TrackerAsset.Verb.Unlocked:
					if (trace.Params.Length > 1 && trace.Params[1].GetType() == typeof(AlternativeTracker.Alternative) && trace.Params[0] is string)
					{
						Tracker.T.Alternative.Unlocked(trace.Key, trace.Params[0].ToString(), (AlternativeTracker.Alternative)Enum.Parse(typeof(AlternativeTracker.Alternative), trace.Params[1].ToString()));
						break;
					}
					if (trace.Params.Length > 0 && trace.Params[0] is string)
					{
						Tracker.T.Alternative.Unlocked(trace.Key, trace.Params[0].ToString());
						break;
					}
					Tracker.T.GameObject.Interacted(trace.Key);
					break;
				case TrackerAsset.Verb.Initialized:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(CompletableTracker.Completable))
					{
						Tracker.T.Completable.Initialized(trace.Key, (CompletableTracker.Completable)Enum.Parse(typeof(CompletableTracker.Completable), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.Completable.Initialized(trace.Key);
					break;
				case TrackerAsset.Verb.Progressed:
					if (trace.Params.Length > 1 && trace.Params[0].GetType() == typeof(CompletableTracker.Completable) && trace.Params[1] is float)
					{
						Tracker.T.Completable.Progressed(trace.Key, (CompletableTracker.Completable)Enum.Parse(typeof(CompletableTracker.Completable), trace.Params[0].ToString()), float.Parse(trace.Params[1].ToString()));
						break;
					}
					if (trace.Params.Length > 0 && trace.Params[0] is float)
					{
						Tracker.T.Completable.Progressed(trace.Key, float.Parse(trace.Params[0].ToString()));
						break;
					}
					Tracker.T.GameObject.Interacted(trace.Key);
					break;
				case TrackerAsset.Verb.Completed:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(CompletableTracker.Completable))
					{
						Tracker.T.Completable.Completed(trace.Key, (CompletableTracker.Completable)Enum.Parse(typeof(CompletableTracker.Completable), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.Completable.Completed(trace.Key);
					break;
				case TrackerAsset.Verb.Interacted:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(GameObjectTracker.TrackedGameObject))
					{
						Tracker.T.GameObject.Interacted(trace.Key, (GameObjectTracker.TrackedGameObject)Enum.Parse(typeof(GameObjectTracker.TrackedGameObject), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.GameObject.Interacted(trace.Key);
					break;
				case TrackerAsset.Verb.Used:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(GameObjectTracker.TrackedGameObject))
					{
						Tracker.T.GameObject.Used(trace.Key, (GameObjectTracker.TrackedGameObject)Enum.Parse(typeof(GameObjectTracker.TrackedGameObject), trace.Params[0].ToString()));
						break;
					}
					Tracker.T.GameObject.Used(trace.Key);
					break;
				default:
					Tracker.T.GameObject.Interacted(trace.Key);
					break;
			}

			Tracker.T.Flush();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	public static void SendEvaluationEvent(TrackerEvalautionEvent ev, Dictionary<TrackerEvaluationKey, string> parameters)
	{
		try
		{
			var valid = false;
			switch (ev)
			{
				case TrackerEvalautionEvent.GameUsage:
				case TrackerEvalautionEvent.UserProfile:
				case TrackerEvalautionEvent.Gamification:
				case TrackerEvalautionEvent.Support:
					valid = parameters.Count == 1 && parameters.Keys.Contains(TrackerEvaluationKey.Event);
					break;
				case TrackerEvalautionEvent.GameActivity:
					valid = parameters.Count == 3 && parameters.Keys.Contains(TrackerEvaluationKey.Event) && parameters.Keys.Contains(TrackerEvaluationKey.GoalOrientation) && parameters.Keys.Contains(TrackerEvaluationKey.Tool);
					break;
				case TrackerEvalautionEvent.GameFlow:
					valid = parameters.Count == 3 && parameters.Keys.Contains(TrackerEvaluationKey.PieceType) && parameters.Keys.Contains(TrackerEvaluationKey.PieceId) && parameters.Keys.Contains(TrackerEvaluationKey.PieceCompleted);
					break;
				case TrackerEvalautionEvent.AssetActivity:
					valid = parameters.Count == 2 && parameters.Keys.Contains(TrackerEvaluationKey.AssetId) && parameters.Keys.Contains(TrackerEvaluationKey.Action);
					break;
				default:
					Debug.LogError("Invalid Evaluation Asset data");
					break;
			}
			if (!valid)
			{
				Debug.LogError("Invalid Evaluation Asset data");
				return;
			}
			var paraString = string.Empty;
			foreach (var para in parameters)
			{
				if (paraString.Length > 0)
				{
					paraString += "&";
				}
				paraString += para.Key.ToString().ToLower();
				paraString += "=";
				paraString += para.Value.ToLower();
			}
			if (!Application.isEditor)
			{
				SendEvaluationEventAsync(ev.ToString().ToLower(), paraString);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	private static async void SendEvaluationEventAsync(string gameEvent, string parameter)
	{
		await Task.Factory.StartNew(() => EvaluationAsset.Instance.sensorData(gameEvent, parameter));
	}
}