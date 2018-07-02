using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;

using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ReviewStateInput : TickStateInput
{
	private readonly string _panelRoute = "ReviewContainer/ReviewPanelContainer";
	private readonly ScenarioController _scenarioController;

	public event Action NextClickedEvent;

	private GameObject _panel;
	private GameObject _background;
	private Image _characterMood;
	private ScrollRect _reviewContent;
	private GameObject _clientChatPrefab;
	private GameObject _playerChatPrefab;
	private GameObject _feedbackPrefab;

	public ReviewStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage");

		_characterMood = GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/StatusBar/MoodPanel/FillBar").GetComponent<Image>();
		_reviewContent = GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/Scroll View").GetComponent<ScrollRect>();
		_clientChatPrefab = Resources.Load<GameObject>("Prefabs/ClientChatFeedbackItem");
		_playerChatPrefab = Resources.Load<GameObject>("Prefabs/PlayerChatFeedbackItem");
		_feedbackPrefab = Resources.Load<GameObject>("Prefabs/FeedbackElement");

		GameObjectUtilities.FindGameObject(_panelRoute + "/ReviewPanel/NextButton").GetComponent<Button>().onClick.AddListener(() => NextClickedEvent?.Invoke());
	}

	protected override void OnEnter()
	{
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "ReviewState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});
		_scenarioController.GetReviewDataSuccessEvent += BuildReviewData;
		CommandQueue.AddCommand(new GetReviewDataCommand());
		_panel.SetActive(true);
		_background.SetActive(true);
	}

	protected override void OnExit()
	{
		_scenarioController.GetReviewDataSuccessEvent -= BuildReviewData;
		_panel.SetActive(false);
		_background.SetActive(false);
	}

	public void BuildReviewData(List<ScenarioController.ChatScoreObject> history, float mood, ScenarioController.FeedbackMode feedbackMode)
	{
		_reviewContent.verticalScrollbar.value = 1;
		_characterMood.fillAmount = (mood + 10) / 20;
		foreach (RectTransform child in _reviewContent.content)
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
			var chatObject = UnityEngine.Object.Instantiate(entryKey == "Client" ? _clientChatPrefab : _playerChatPrefab, _reviewContent.content, false);
			chatObject.transform.Find("Panel/BubbleText").GetComponent<Text>().text = history[i].ChatObject.Utterence;
			if (entryKey == "Player" && feedbackMode >= ScenarioController.FeedbackMode.EndGame && history[i] != null && history[i].Scores.Count > 0)
			{
				var feedbackPanel = chatObject.transform.Find("FeedbackPanel");
				var scoreWidth = 0f;
				foreach (var feedbackScore in history[i].Scores)
				{
					if (feedbackPanel.GetComponent<LayoutGroup>().minWidth + scoreWidth > _reviewContent.content.rect.width)
					{
						var newFeedbackPanel = UnityEngine.Object.Instantiate(feedbackPanel, chatObject.transform, false);
						foreach (RectTransform child in newFeedbackPanel)
						{
							UnityEngine.Object.Destroy(child.gameObject);
						}
						feedbackPanel = newFeedbackPanel;
					}
					var score = UnityEngine.Object.Instantiate(_feedbackPrefab, feedbackPanel, false);
					score.transform.FindText("Title").text = Localization.Get("POINTS_" + feedbackScore.Key.ToUpper());
					score.transform.FindText("Value").text = feedbackScore.Value > 0 ? "+" + feedbackScore.Value : feedbackScore.Value.ToString();
					scores.Add(score);
					score.BestFit();
					scoreWidth = score.RectTransform().rect.width;
				}
			}
		}
		scores.BestFit();
	}
}
