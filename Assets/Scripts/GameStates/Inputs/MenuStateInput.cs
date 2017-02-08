﻿using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
	public class MenuStateInput : TickStateInput
	{
		public event Action SettingsClickedEvent;
		public event Action PlayClickedEvent;

		private ButtonList _buttons;
		private Button _playButton;

		protected override void OnInitialize()
		{
			_buttons = new ButtonList("MenuContainer/MenuPanelContainer/MenuPanel");
			_playButton = _buttons.GetButton("PlayButton");
			_playButton.onClick.AddListener(OnPlayClick);
			var settingsButton = _buttons.GetButton("SettingsButton");
			settingsButton.onClick.AddListener(OnSettingsClick);
			var leaderboardButton = _buttons.GetButton("LeaderboardButton");
			leaderboardButton.onClick.AddListener(delegate
			{
				SUGARManager.Leaderboard.Display("smi_stars", LeaderboardFilterType.Near);
			});
		}

		private void OnSettingsClick()
		{
			SettingsClickedEvent();
		}

		protected override void OnEnter()
		{
			Tracker.T.accessible.Accessed("MainMenu", AccessibleTracker.Accessible.Screen);
			_buttons.GameObjects.BestFit();
			GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(true);
			GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(true);
		}

		protected override void OnExit()
		{
			GameObjectUtilities.FindGameObject("MenuContainer/MenuPanelContainer").SetActive(false);
			GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
		}

		private void OnPlayClick()
		{
			PlayClickedEvent();
		}
	}
}
