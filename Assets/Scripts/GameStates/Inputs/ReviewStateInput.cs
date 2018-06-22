using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;

using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ReviewStateInput : TickStateInput
{
	private readonly string _panelRoute = "ReviewContainer/ReviewPanelContainer";

	public event Action NextClickedEvent;

	private Image _characterMood;
	private GameObject _reviewContent;
	private GameObject _clientChatPrefab;
	private GameObject _playerChatPrefab;
	private GameObject _feedbackPrefab;
	private readonly ScenarioController _scenarioController;

	public ReviewStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_characterMood = GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/StatusBar/MoodPanel/FillBar").GetComponent<Image>();
		_reviewContent = GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/Scroll View");
		_clientChatPrefab = Resources.Load("Prefabs/ClientChatFeedbackItem") as GameObject;
		_playerChatPrefab = Resources.Load("Prefabs/PlayerChatFeedbackItem") as GameObject;
		_feedbackPrefab = Resources.Load("Prefabs/FeedbackElement") as GameObject;

		GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/NextButton").GetComponent<Button>().onClick.AddListener(() => NextClickedEvent?.Invoke());
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
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
	}

	protected override void OnExit()
	{
		_scenarioController.GetReviewDataSuccessEvent -= BuildReviewData;
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
	}

	public void BuildReviewData(List<ScenarioController.ChatScoreObject> history, float mood, ScenarioController.FeedbackMode feedbackMode)
	{
		_reviewContent.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
		_characterMood.fillAmount = (mood + 10) / 20;
        foreach (RectTransform child in _reviewContent.GetComponent<ScrollRect>().content)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

		var scores = new List<GameObject>();

		for (var i = 0; i < history.Count; i++)
		{
			var entryKey = history[i].ChatObject.Agent;
			if (entryKey == "Player" && i + 1 < history.Count)
			{
				foreach (var chatScore in history[i + 1].Scores)
				{
					if (history[i].Scores.ContainsKey(chatScore.Key))
					{
						history[i].Scores[chatScore.Key] = chatScore.Value;
					}
					else
					{
						history[i].Scores.Add(chatScore.Key, chatScore.Value);
					}
				}
			}
			var chatObject = UnityEngine.Object.Instantiate(entryKey == "Client" ? _clientChatPrefab : _playerChatPrefab, _reviewContent.GetComponent<ScrollRect>().content, false);
            chatObject.transform.Find("Panel").GetChild(0).GetComponent<Text>().text = history[i].ChatObject.Utterence;
            if (entryKey == "Player" && (int)feedbackMode >= 1 && history[i] != null && history[i].Scores.Count > 0)
			{
                var feedbackPanel = chatObject.transform.Find("FeedbackPanel").transform;
				var scoreWidth = 0f;
                foreach (var feedbackScore in history[i].Scores)
                {
					if (feedbackPanel.GetComponent<LayoutGroup>().minWidth + scoreWidth > _reviewContent.GetComponent<ScrollRect>().content.rect.width)
					{
						var newFeedbackPanel = UnityEngine.Object.Instantiate(feedbackPanel, chatObject.transform, false);
						foreach (RectTransform child in newFeedbackPanel)
						{
							UnityEngine.Object.Destroy(child.gameObject);
						}
						feedbackPanel = newFeedbackPanel;
					}
                    var score = UnityEngine.Object.Instantiate(_feedbackPrefab, feedbackPanel, false);
                    score.transform.Find("Title").GetComponent<Text>().text = Localization.Get("POINTS_" + feedbackScore.Key.ToUpper());
                    score.transform.Find("Value").GetComponent<Text>().text = feedbackScore.Value > 0 ? "+" + feedbackScore.Value : feedbackScore.Value.ToString();
					scores.Add(score);
					score.BestFit();
					scoreWidth = ((RectTransform)score.transform).rect.width;
				}
            }
		}
		scores.BestFit();
	}
}
