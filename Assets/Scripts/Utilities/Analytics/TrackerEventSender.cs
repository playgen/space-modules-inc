using System;
using System.Collections.Generic;
using System.Linq;
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
public class TrackerEventSender {
	public static void SendEvent(TraceEvent trace)
	{
		try
		{
			foreach (var v in trace.Values.OrderBy(v => v.Key))
			{
				if (v.Key == TrackerContextKeys.TriggerUI.ToString())
				{
					if (string.IsNullOrEmpty(v.Value))
					{
						Debug.LogWarning(trace.Key + " event not tracked due to null TriggerUI value. If not caused by internal game event, this is a bug!");
						return;
					}
					if (!Enum.IsDefined(typeof(TrackerTriggerSources), v.Value))
					{
						Debug.LogWarning("TrackerTriggerSources does not contain key " + v.Value + ". This is likely due to a typo in the inspector.");
					}
				}
				Tracker.T.setVar(v.Key, v.Value);
			}
			if (SUGARManager.CurrentUser != null)
			{
				Tracker.T.setVar("CurrentUser", SUGARManager.CurrentUser.Name);
			}
			switch (trace.ActionType)
			{
				case TrackerAsset.Verb.Accessed:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(AccessibleTracker.Accessible))
					{
						Tracker.T.Accessible.Accessed(trace.Key, (AccessibleTracker.Accessible)Enum.Parse(typeof(AccessibleTracker.Accessible), trace.Params[0].ToString()));
						//TODO need to review this logic for SMI
						if ((AccessibleTracker.Accessible)Enum.Parse(typeof(AccessibleTracker.Accessible), trace.Params[0].ToString()) == AccessibleTracker.Accessible.Accessible)
						{
							SendEvaluationEvent(TrackerEvalautionEvents.Support, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key } });
						}
						else
						{
							//TODO Need to work out what should replace 'tool'
							SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key }, { TrackerEvaluationKeys.GoalOrientation, "neutral" }, { TrackerEvaluationKeys.Tool, "tool" } });
						}
						break;
					}
					Tracker.T.Accessible.Accessed(trace.Key);
					break;
				case TrackerAsset.Verb.Skipped:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(AccessibleTracker.Accessible))
					{
						Tracker.T.Accessible.Skipped(trace.Key, (AccessibleTracker.Accessible)Enum.Parse(typeof(AccessibleTracker.Accessible), trace.Params[0].ToString()));
						//TODO Need to work out what should replace 'tool'
						SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key }, { TrackerEvaluationKeys.GoalOrientation, "neutral" }, { TrackerEvaluationKeys.Tool, "tool" } });
						break;
					}
					Tracker.T.Accessible.Skipped(trace.Key);
					break;
				case TrackerAsset.Verb.Selected:
					if (trace.Params.Length > 1 && trace.Params[1].GetType() == typeof(AlternativeTracker.Alternative) && trace.Params[0] is string)
					{
						Tracker.T.Alternative.Selected(trace.Key, trace.Params[0].ToString(), (AlternativeTracker.Alternative)Enum.Parse(typeof(AlternativeTracker.Alternative), trace.Params[1].ToString()));
						//TODO Need to work out what should replace 'tool'
						SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key }, { TrackerEvaluationKeys.GoalOrientation, "neutral" }, { TrackerEvaluationKeys.Tool, "tool" } });
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
						SendEvaluationEvent(TrackerEvalautionEvents.GameUsage, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key } });
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
						//TODO Scenario needs to be replaced with the current level number
						SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Type, trace.Key }, { TrackerEvaluationKeys.Id, "scenario" }, { TrackerEvaluationKeys.Completed, "success" } });
						break;
					}
					Tracker.T.Completable.Completed(trace.Key);
					break;
				case TrackerAsset.Verb.Interacted:
					if (trace.Params.Length > 0 && trace.Params[0].GetType() == typeof(GameObjectTracker.TrackedGameObject))
					{
						Tracker.T.GameObject.Interacted(trace.Key, (GameObjectTracker.TrackedGameObject)Enum.Parse(typeof(GameObjectTracker.TrackedGameObject), trace.Params[0].ToString()));
						//TODO Need to work out what should replace 'tool'
						SendEvaluationEvent(TrackerEvalautionEvents.GameActivity, new Dictionary<TrackerEvaluationKeys, string> { { TrackerEvaluationKeys.Event, trace.Key }, { TrackerEvaluationKeys.GoalOrientation, "neutral" }, { TrackerEvaluationKeys.Tool, "tool" } });
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

	public static void SendEvaluationEvent(TrackerEvalautionEvents ev, Dictionary<TrackerEvaluationKeys, string> parameters)
	{
		try
		{
			var valid = false;
			switch (ev)
			{
				case TrackerEvalautionEvents.GameUsage:
					valid = (parameters.Count == 1 && parameters.Keys.Contains(TrackerEvaluationKeys.Event));
					break;
				case TrackerEvalautionEvents.UserProfile:
					valid = (parameters.Count == 1 && parameters.Keys.Contains(TrackerEvaluationKeys.Event));
					break;
				case TrackerEvalautionEvents.GameActivity:
					valid = (parameters.Count == 3 && parameters.Keys.Contains(TrackerEvaluationKeys.Event) && parameters.Keys.Contains(TrackerEvaluationKeys.GoalOrientation) && parameters.Keys.Contains(TrackerEvaluationKeys.Tool));
					break;
				case TrackerEvalautionEvents.Gamification:
					valid = (parameters.Count == 1 && parameters.Keys.Contains(TrackerEvaluationKeys.Event));
					break;
				case TrackerEvalautionEvents.GameFlow:
					valid = (parameters.Count == 3 && parameters.Keys.Contains(TrackerEvaluationKeys.Type) && parameters.Keys.Contains(TrackerEvaluationKeys.Id) && parameters.Keys.Contains(TrackerEvaluationKeys.Completed));
					break;
				case TrackerEvalautionEvents.Support:
					valid = (parameters.Count == 1 && parameters.Keys.Contains(TrackerEvaluationKeys.Event));
					break;
				case TrackerEvalautionEvents.AssetActivity:
					valid = (parameters.Count == 2 && parameters.Keys.Contains(TrackerEvaluationKeys.Asset) && parameters.Keys.Contains(TrackerEvaluationKeys.Done));
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
			EvaluationAsset.Instance.sensorData(ev.ToString().ToLower(), paraString);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}
}