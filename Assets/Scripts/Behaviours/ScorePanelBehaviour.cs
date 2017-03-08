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

    public void SetScorePanel(int stars, int score, string scoreFeedbackToken, bool moodImage, string emotionCommentToken, int bonus)
    {
        // set stars
        foreach (GameObject starSlot in _starSlots)
        {
            if (stars > 0)
            {
                starSlot.GetComponent<Image>().sprite = _starSprite;
                stars--;
            }
            else
            {
                starSlot.GetComponent<Image>().sprite = _starEmptySprite;
            }
        }
        
        _scoreText.text = score.ToString("N0");


        _scoreFeedbackText.text = Localization.Get(scoreFeedbackToken);

        _emotionImage.sprite = moodImage ? _emotionPositiveSprite : _emotionNegativeSprite;

        _emotionComment.text = Localization.Get(emotionCommentToken);

        _bonusText.text = "+" + bonus.ToString("N0");
    }

}
