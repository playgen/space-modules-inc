using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

public class CallStateInput : TickStateInput
{
	private readonly string _panelRoute = "CallContainer/CallPanelContainer";
	public event Action AnswerClickedEvent;

	protected override void OnInitialize()
	{
		GameObjectUtilities.FindGameObject(_panelRoute + "/AnswerButton").GetComponent<Button>().onClick.AddListener(OnAnswerClick);
	}

	private void OnAnswerClick()
	{
		AnswerClickedEvent?.Invoke();
	}

	protected override void OnEnter()
	{
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
		GameObjectUtilities.FindGameObject(_panelRoute + "/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StartAnimation();
		GameObjectUtilities.FindGameObject(_panelRoute).BestFit();
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
		GameObjectUtilities.FindGameObject(_panelRoute + "/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StopAnimation();

	}
}