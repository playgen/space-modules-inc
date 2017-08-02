using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using UnityEngine;
using UnityEngine.UI;

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
		nextButton.GetComponent<Button>().onClick.AddListener(() => {
			if (NextClickedEvent != null) NextClickedEvent();
		});
	}

	protected override void OnEnter()
	{
		Tracker.T.accessible.Accessed("ReviewState", AccessibleTracker.Accessible.Screen);
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

		foreach (ScenarioController.ChatObject t in history)
		{
			Transform chatObject = null;

			var entryKey = t.Agent;
			switch (entryKey)
			{
				case "Client":
					chatObject = UnityEngine.Object.Instantiate(_clientChatPrefab).transform;
					break;
				case "Player":
					chatObject = UnityEngine.Object.Instantiate(_playerChatPrefab).transform;
					break;
			}

			if (chatObject != null)
			{
				chatObject.Find("Panel").GetChild(0).GetComponent<Text>().text = t.Utterence;
				chatObject.transform.SetParent(_reviewContent.GetComponent<ScrollRect>().content, false);
			}
		}
	}

	private void ClearList()
	{
		foreach (RectTransform child in _reviewContent.GetComponent<ScrollRect>().content)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}
}
