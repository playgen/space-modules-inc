using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;
using RAGE.EvaluationAsset;
using UnityEngine;

public class LoadingStateInput : TickStateInput
{
	public event Action LoggedInEvent;

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
	}

	protected override void OnTick(float deltaTime) {
		// Check for SUGAR login
		SUGARManager.Unity.gameObject.GetComponent<AccountUnityClientAdditions>().DisplayPanel(success =>
		{
			if (success)
			{
				var settings = new EvaluationAssetSettings { PlayerId = SUGARManager.CurrentUser.Name };
				EvaluationAsset.Instance.Settings = settings;
				TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameUsage, new Dictionary<TrackerEvaluationKeys, string>
				{
					{ TrackerEvaluationKeys.Event, "GameStart" }
					
				});
				TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.UserProfile, new Dictionary<TrackerEvaluationKeys, string>
				{
					{ TrackerEvaluationKeys.Event, "SUGARSignIn" }
				});
				TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.AssetActivity, new Dictionary<TrackerEvaluationKeys, string>
				{
					{ TrackerEvaluationKeys.Asset, "SUGAR" },
					{ TrackerEvaluationKeys.Done, "true" }
				});
			}
			LoggedInEvent?.Invoke();
		});
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
	}
}