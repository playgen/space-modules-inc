using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.Unity.Utilities.Localization;
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
		nextButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				NextClickedEvent?.Invoke();
			});
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "ReviewState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
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

						foreach (var feedbackScore in feedback.Scores)
						{
							var score = UnityEngine.Object.Instantiate(_feedbackPrefab).transform;
							score.transform.SetParent(feedbackPanel, false);
							score.transform.Find("Title").GetComponent<Text>().text = Localization.Get("POINTS_" + feedbackScore.Key.ToUpper());
							score.transform.Find("Value").GetComponent<Text>().text = feedbackScore.Value > 0 ? "+" + feedbackScore.Value : feedbackScore.Value.ToString();

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
