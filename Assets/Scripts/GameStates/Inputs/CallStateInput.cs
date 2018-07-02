using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;

using UnityEngine;

public class CallStateInput : TickStateInput
{
	private readonly string _panelRoute = "CallContainer/CallPanelContainer";
	public event Action AnswerClickedEvent;

	private GameObject _panel;
	private GameObject _background;
	private CallAnimationBehaviour _callAnim;

	protected override void OnInitialize()
	{
		GameObjectUtilities.FindGameObject(_panelRoute + "/AnswerButton").GetComponent<Button>().onClick.AddListener(() => AnswerClickedEvent?.Invoke());
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage");
		_callAnim = GameObjectUtilities.FindGameObject(_panelRoute + "/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>();
	}

	protected override void OnEnter()
	{
		_panel.SetActive(true);
		_background.SetActive(true);
		_callAnim.StartAnimation();
		_panel.BestFit();
	}

	protected override void OnExit()
	{
		_panel.SetActive(false);
		_background.SetActive(false);
		_callAnim.StopAnimation();
	}
}