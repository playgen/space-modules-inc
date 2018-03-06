using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.Unity.Utilities.BestFit;
using UnityEngine;
using UnityEngine.UI;

public class ReviewStateInput : TickStateInput
{
	public event Action NextClickedEvent;

	private Image _characterMood;
	private GameObject _reviewContent;
	private GameObject _clientChatPrefab;
	private GameObject _playerChatPrefab;
	private GameObject _playerChatFeedbackPrefab;
	private GameObject _feedbackPrefab;
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
		_playerChatFeedbackPrefab = Resources.Load("Prefabs/PlayerChatFeedbackItem") as GameObject;
		_feedbackPrefab = Resources.Load("Prefabs/FeedbackElement") as GameObject;

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

	public void BuildReviewData(List<ScenarioController.ChatScoreObject> history, float mood, ScenarioController.FeedbackMode feedbackMode)
	{
		_characterMood.fillAmount = (mood + 10) / 20;
		ClearList();

		foreach (var t in history)
		{
			Transform chatObject = null;

			var entryKey = t.ChatObject.Agent;
			switch (entryKey)
			{
				case "Client":
					chatObject = UnityEngine.Object.Instantiate(_clientChatPrefab).transform;
					break;
				case "Player":
					var feedback = history.Find(c => c.ChatObject == t.ChatObject);
					
					if ((int)feedbackMode >= 1 && feedback != null && feedback.Scores.Count > 0)
					{
						chatObject = UnityEngine.Object.Instantiate(_playerChatFeedbackPrefab).transform;
						var feedbackPanel = chatObject.transform.Find("FeedbackPanel").transform;

						var rect = _reviewContent.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().rect;
						var width = rect.width / feedback.Scores.Count;
						width = Mathf.Min(width, _feedbackPrefab.GetComponent<RectTransform>().rect.width);

						foreach (var feedbackScore in feedback.Scores)
						{
							var score = UnityEngine.Object.Instantiate(_feedbackPrefab).transform;
							score.transform.SetParent(feedbackPanel, false);
							var change = feedbackScore.Value;
							score.GetComponentInChildren<Text>().text = change > 0 ? "+" + change : change.ToString();

							score.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width/2);


							var iconPath = "Prefabs/Icons/" + feedbackScore.Key;
							var icon = Resources.Load<Sprite>(iconPath);
							score.GetComponentInChildren<Image>().sprite = icon;
						}
						
					}
					else
					{
						chatObject = UnityEngine.Object.Instantiate(_playerChatPrefab).transform;
					}
					break;
			}

			if (chatObject != null)
			{
				chatObject.Find("Panel").GetChild(0).GetComponent<Text>().text = t.ChatObject.Utterence;
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
