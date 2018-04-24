using System.Collections.Generic;
using System.Linq;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanelBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject _finalScorePanel;

	[SerializeField]
	private GameObject _finalScoreFeedbackPanel;

	[SerializeField]
	private Sprite _starSprite;

	[SerializeField]
	private Sprite _starEmptySprite;

	[SerializeField]
	private GameObject[] _starSlots;

	[SerializeField]	
	private Image _emotionImage;

	[SerializeField]
	private Text _emotionComment;

	[SerializeField]
	private Sprite _emotionPositiveSprite;

	[SerializeField]
	private Sprite _emotionNegativeSprite;

	private GameObject _feedbackElementGameObject;
	private Transform _feedbackPanel;
	private Text _scoreText;
	private Text _scoreFeedbackText;
	private GameObject _scorePanel;

	public void SetScorePanel(ScenarioController.ScoreObject score)
	{
		_finalScorePanel.SetActive(false);
		_finalScoreFeedbackPanel.SetActive(false);

		_scorePanel = score.MeasuredPoints.Any() ? _finalScoreFeedbackPanel : _finalScorePanel;
		_scorePanel.SetActive(true);

		_scoreText = _scorePanel.transform.Find("ScoreText").GetComponent<Text>();
		_scoreFeedbackText = _scorePanel.transform.Find("ScoreFeedback").GetComponent<Text>();

		SetupFeedback();
		SetFeedbackIcons(score.MeasuredPoints);
		
		// set stars
		foreach (var starSlot in _starSlots)
		{
			if (score.Stars > 0)
			{
				starSlot.GetComponent<Image>().sprite = _starSprite;
				score.Stars--;
			}
			else
			{
				starSlot.GetComponent<Image>().sprite = _starEmptySprite;
			}
		}
		
		_scoreText.text = score.Score.ToString("N0");

		_scoreFeedbackText.text = Localization.Get(score.ScoreFeedbackToken);

		_emotionImage.sprite = score.MoodImage ? _emotionPositiveSprite : _emotionNegativeSprite;

		_emotionComment.text = Localization.Get(score.EmotionCommentToken);
	}

	private void SetupFeedback()
	{
		if (_feedbackPanel == null)
		{
			_feedbackPanel = _finalScoreFeedbackPanel.transform.Find("FeedbackPanel").GetComponent<FeedbackHelper>().transform;
		}
		if (_feedbackElementGameObject == null)
		{
			_feedbackElementGameObject = Resources.Load("Prefabs/FeedbackElement") as GameObject;
		}
	}

	private void SetFeedbackIcons(Dictionary<string, int> points)
	{
		var feedbackHelper = _feedbackPanel.GetComponent<FeedbackHelper>();

		foreach (var point in points)
		{
			var value = feedbackHelper.GetTextComponent(point.Key);

			value.text = point.Value > 0 ? "+" + point.Value : point.Value.ToString();
		}
		
		_feedbackPanel.BestFit();
		feedbackHelper.DoBestFit();
	}

}
