using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
	public class LoadingStateInput : TickStateInput
	{
		public event Action OfflineClickedEvent;
		public event Action LoggedInEvent;

		protected override void OnInitialize()
		{
			GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer/OfflineButton")
				.GetComponent<Button>()
				.onClick.AddListener(() => OfflineClickedEvent());

		}

		protected override void OnEnter()
		{
			GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(true);
			// Check for SUGAR login
			SUGARManager.Account.DisplayPanel(success =>
			{
				if (success)
				{
					LoggedInEvent();
				}
				else
				{
					Debug.LogError("Sign In FAAILL");
				}
			});
		}

		protected override void OnExit()
		{
			GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
		}
	}
}
