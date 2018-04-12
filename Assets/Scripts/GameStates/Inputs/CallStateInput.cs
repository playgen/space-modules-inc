﻿using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

using RAGE.Analytics;

public class CallStateInput : TickStateInput
{
	public event Action AnswerClickedEvent;

	protected override void OnInitialize()
	{
		var answerButton = GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/AnswerButton").GetComponent<Button>();
		answerButton.onClick.AddListener(OnAnswerClick);
	}

	private void OnAnswerClick()
	{
		if (AnswerClickedEvent != null) AnswerClickedEvent();
	}

	protected override void OnEnter()
	{
		Tracker.T.Accessible.Accessed("CallState", AccessibleTracker.Accessible.Screen);
		GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
		GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StartAnimation();
		GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").BestFit();
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
		GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StopAnimation();

	}
}