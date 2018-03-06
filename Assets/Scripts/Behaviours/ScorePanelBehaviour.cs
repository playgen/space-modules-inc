using System.Collections.Generic;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanelBehaviour : MonoBehaviour
{
    [SerializeField]
    private Sprite _starSprite;

    [SerializeField]
    private Sprite _starEmptySprite;

    [SerializeField]
    private GameObject[] _starSlots;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _scoreFeedbackText;

    [SerializeField]
    private Image _emotionImage;

    [SerializeField]
    private Text _emotionComment;

    [SerializeField]
    private Sprite _emotionPositiveSprite;

    [SerializeField]
    private Sprite _emotionNegativeSprite;

    [SerializeField]
    private Text _bonusText;

	private GameObject _feedbackElementGameObject;
	private Transform _feedbackPanel;
	private List<GameObject> _feedbackElements = new List<GameObject>();

    public void SetScorePanel(ScenarioController.ScoreObject score)
    {
	    SetupFeedback();
	    SetFeedbackIcons(score.MeasuredPoints);
		
		// set stars
		foreach (GameObject starSlot in _starSlots)
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

        _emotionImage.sprite = score.MoodImage? _emotionPositiveSprite : _emotionNegativeSprite;

        _emotionComment.text = Localization.Get(score.EmotionCommentToken);

        _bonusText.text = "+" + score.Bonus.ToString("N0");
    }

	private void SetupFeedback()
	{
		if (_feedbackPanel == null)
		{
			_feedbackPanel = GameObjectUtilities
				.Find("ScoreContainer/ScorePanelContainer/ScorePanel/TopPanel/FinalScorePanel/FeedbackPanel").transform;
		}
		if (_feedbackElementGameObject == null)
		{
			_feedbackElementGameObject = Resources.Load("Prefabs/FeedbackElement") as GameObject;
		}

		if (_feedbackElements.Count > 0)
		{
			foreach (var feedbackElement in _feedbackElements)
			{
				DestroyImmediate(feedbackElement);
			}
		}
	}

	private void SetFeedbackIcons(Dictionary<string, int> points)
	{
		_feedbackElements.Clear();
		foreach (var point in points)
		{
			var element = Object.Instantiate(_feedbackElementGameObject);
			element.transform.SetParent(_feedbackPanel.transform, false);

			var icon = Resources.Load<Sprite>("Prefabs/Icons/" + point.Key);
			element.GetComponentInChildren<Image>().sprite = icon;
			element.GetComponentInChildren<Text>().text = point.Value > 0 ? "+" + point.Value : point.Value.ToString();
			_feedbackElements.Add(element);
		}
	}

}
