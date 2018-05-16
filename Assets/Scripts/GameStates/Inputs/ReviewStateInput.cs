﻿using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;

using PlayGen.Unity.Utilities.BestFit;
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

		GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/NextButton").GetComponent<Button>().onClick.AddListener(() => NextClickedEvent?.Invoke());
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
		if (_scenarioController.LevelMax > 0)
		{
			PlayerPrefs.SetInt("CurrentLevel", _scenarioController.CurrentLevel);
		}
	}

	protected override void OnExit()
	{
		_scenarioController.GetReviewDataSuccessEvent -= BuildReviewData;
		GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
	}

	public void BuildReviewData(List<ScenarioController.ChatScoreObject> history, float mood, ScenarioController.FeedbackMode feedbackMode)
	{
		_reviewContent.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
		_characterMood.fillAmount = (mood + 10) / 20;
        foreach (RectTransform child in _reviewContent.GetComponent<ScrollRect>().content)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

		var scores = new List<GameObject>();
        foreach (var t in history)
		{
			var entryKey = t.ChatObject.Agent;
            var feedback = history.Find(c => c.ChatObject == t.ChatObject);
            var chatObject = UnityEngine.Object.Instantiate(entryKey == "Client" ? _clientChatPrefab : ((int)feedbackMode >= 1 && feedback != null && feedback.Scores.Count > 0) ? _playerChatFeedbackPrefab : _playerChatPrefab, _reviewContent.GetComponent<ScrollRect>().content, false);
            chatObject.transform.Find("Panel").GetChild(0).GetComponent<Text>().text = t.ChatObject.Utterence;
            if (entryKey == "Player" && (int)feedbackMode >= 1 && feedback != null && feedback.Scores.Count > 0)
			{
                var feedbackPanel = chatObject.transform.Find("FeedbackPanel").transform;
                foreach (var feedbackScore in feedback.Scores)
                {
                    var score = UnityEngine.Object.Instantiate(_feedbackPrefab, feedbackPanel, false);
                    score.transform.Find("Title").GetComponent<Text>().text = Localization.Get("POINTS_" + feedbackScore.Key.ToUpper());
                    score.transform.Find("Value").GetComponent<Text>().text = feedbackScore.Value > 0 ? "+" + feedbackScore.Value : feedbackScore.Value.ToString();
					scores.Add(score);
				}
            }
		}
		scores.BestFit();
	}
}
