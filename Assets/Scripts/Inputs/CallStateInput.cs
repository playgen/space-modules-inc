using System;
using GameWork.Core.States.Tick.Input;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
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
			AnswerClickedEvent();
		}

		protected override void OnEnter()
		{
			GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(true);
			GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
			GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StartAnimation();
		}

		protected override void OnExit()
		{
			GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer").SetActive(false);
			GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
			GameObjectUtilities.FindGameObject("CallContainer/CallPanelContainer/SatelliteAnimContainer").GetComponent<CallAnimationBehaviour>().StopAnimation();

		}
	}
}
