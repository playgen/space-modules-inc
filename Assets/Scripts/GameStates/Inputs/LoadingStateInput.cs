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

	private GameObject _splash;

	protected override void OnInitialize()
	{
		_splash = GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer");
	}

	protected override void OnEnter()
	{
		_splash.SetActive(true);
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
		_splash.SetActive(false);
	}
}