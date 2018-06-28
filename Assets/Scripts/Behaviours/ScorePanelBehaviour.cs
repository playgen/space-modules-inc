using System.Collections.Generic;
using System.Linq;

using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Text;
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
	private Image[] _starSlots;

	[SerializeField]	
	private Image _emotionImage;

	[SerializeField]
	private Text _emotionComment;

	[SerializeField]
	private Sprite _emotionPositiveSprite;

	[SerializeField]
	private Sprite _emotionNegativeSprite;

	private GameObject _feedbackElementGameObject;
	private FeedbackHelper _feedbackPanel;
	private Text _scoreText;
	private Text _scoreFeedbackText;
	private GameObject _scorePanel;

	public void SetScorePanel(ScenarioController.ScoreObject score)
	{
		_finalScorePanel.SetActive(false);
		_finalScoreFeedbackPanel.SetActive(false);

		_scorePanel = score.MeasuredPoints.Any() ? _finalScoreFeedbackPanel : _finalScorePanel;
		_scorePanel.SetActive(true);

		_scoreText = _scorePanel.transform.FindText("ScoreText");
		_scoreFeedbackText = _scorePanel.transform.FindText("ScoreFeedback");

		SetupFeedback();
		SetFeedbackIcons(score.MeasuredPoints);

		// set stars
		for (var i = 0; i < _starSlots.Length; i++)
		{
			_starSlots[i].sprite = score.Stars > i ? _starSprite : _starEmptySprite;
		}
		
		_scoreText.text = score.Score.ToString("N0", Localization.SpecificSelectedLanguage);
		_scoreFeedbackText.text = Localization.Get(score.ScoreFeedbackToken);
		_emotionImage.sprite = score.MoodImage ? _emotionPositiveSprite : _emotionNegativeSprite;
		_emotionComment.text = Localization.Get(score.EmotionCommentToken);
	}

	private void SetupFeedback()
	{
		if (!_feedbackPanel)
		{
			_feedbackPanel = _finalScoreFeedbackPanel?.GetComponentInChildren<FeedbackHelper>(true);
		}
		if (!_feedbackElementGameObject)
		{
			_feedbackElementGameObject = Resources.Load<GameObject>("Prefabs/FeedbackElement");
		}
	}

	private void SetFeedbackIcons(Dictionary<string, int> points)
	{
		foreach (var point in points)
		{
			var value = _feedbackPanel.GetTextComponent(point.Key);
			value.text = point.Value > 0 ? "+" + point.Value : point.Value.ToString();
		}
		_feedbackPanel.BestFit();
	}
}
