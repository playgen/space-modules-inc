using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;
using UnityEngine;

public class LoadingStateInput : TickStateInput
{
	public event Action LoggedInEvent;
	private float _timeSinceStart;
	private float _signInTriggerTime;

	protected override void OnInitialize()
	{
		var backgrounds = GameObjectUtilities.FindGameObject("BackgroundContainer");
		var aspect = Camera.main.aspect;
		var backgroundSize = (3f / 4f) / aspect;
		backgroundSize = backgroundSize < 1 ? 1 : backgroundSize;
		((RectTransform) backgrounds.transform).anchorMin = new Vector2((1 - backgroundSize) * 0.5f, 0);
		((RectTransform) backgrounds.transform).anchorMax = new Vector2(((backgroundSize - 1) * 0.5f) + 1, 1);
	}

	protected override void OnEnter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(true);
		_timeSinceStart = 0;
		SUGARManager.Unity.StartSpinner();
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_signInTriggerTime = 1;
		}
	}

	protected override void OnTick(float deltaTime)
	{
		if (_timeSinceStart <= _signInTriggerTime)
		{
			_timeSinceStart += deltaTime;
			if (_timeSinceStart > _signInTriggerTime)
			{
				// Check for SUGAR login
				SUGARManager.Unity.gameObject.GetComponent<AccountUnityClientAdditions> ().DisplayPanel (success =>
				{
					if (success)
					{
						TrackerEventSender.SendEvaluationEvent (TrackerEvalautionEvent.GameUsage, new Dictionary<TrackerEvaluationKey, string>
						{
							{ TrackerEvaluationKey.Event, "GameStart" }
						});
						TrackerEventSender.SendEvaluationEvent (TrackerEvalautionEvent.UserProfile, new Dictionary<TrackerEvaluationKey, string>
						{
							{ TrackerEvaluationKey.Event, "SUGARSignIn" }
						});
						TrackerEventSender.SendEvaluationEvent (TrackerEvalautionEvent.AssetActivity, new Dictionary<TrackerEvaluationKey, string> 
						{
							{ TrackerEvaluationKey.AssetId, "SUGAR" },
							{ TrackerEvaluationKey.Action, "SignIn" }
						});
					}
					SUGARManager.Unity.StopSpinner();
					LoggedInEvent?.Invoke ();
				});
			}
		}
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
	}
}