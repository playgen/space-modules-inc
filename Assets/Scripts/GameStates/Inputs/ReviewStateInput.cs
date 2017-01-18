using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
	public class ReviewStateInput : TickStateInput
	{
		public event Action NextClickedEvent;

		private Image _characterMood;
		private GameObject _reviewContent;
		private GameObject _clientChatPrefab;
		private GameObject _playerChatPrefab;
		private readonly ScenarioController _scenarioController;

		public ReviewStateInput(ScenarioController scenarioController)
		{
			_scenarioController = scenarioController;
		}

		protected override void OnInitialize()
		{
			_characterMood = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/StatusBar/MoodPanel/FillBar").GetComponent<Image>();
			_reviewContent = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/Scroll View");
			_clientChatPrefab = Resources.Load("Prefabs/ClientChatItem") as GameObject;
			_playerChatPrefab = Resources.Load("Prefabs/PlayerChatItem") as GameObject;

			var nextButton =
				GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/NextButton");
			nextButton.GetComponent<Button>().onClick.AddListener(() => NextClickedEvent());
		}
	
		protected override void OnEnter()
		{
			_scenarioController.GetReviewDataSuccessEvent += BuildReviewData;
			CommandQueue.AddCommand(new GetReviewDataCommand());
			GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(true);
			GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
		}

		protected override void OnExit()
		{
			_scenarioController.GetReviewDataSuccessEvent -= BuildReviewData;
			GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(false);
			GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
		}

		public void BuildReviewData(List<ScenarioController.ChatObject> history, float mood)
		{
			_characterMood.fillAmount = (mood + 10) / 20;
			ClearList();

			//string[] dialogues = new string[history.Count];
			//history.Keys.CopyTo(dialogues,0);

			for (var i = 0; i < history.Count; i++)
			{
				Transform chatObject = null;

				var entryKey = history[i].Agent;
				if (entryKey == "Client")
				{
					chatObject = GameObject.Instantiate(_clientChatPrefab).transform;
				}
				else if (entryKey == "Player")
				{
					chatObject = GameObject.Instantiate(_playerChatPrefab).transform;
				}

				if (chatObject != null)
				{
					chatObject.FindChild("Panel").GetChild(0).GetComponent<Text>().text = history[i].Utterence;
					chatObject.transform.SetParent(_reviewContent.GetComponent<ScrollRect>().content, false);
				}
			}
		}

		private void ClearList()
		{
			foreach (RectTransform child in _reviewContent.GetComponent<ScrollRect>().content)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
	}
}
