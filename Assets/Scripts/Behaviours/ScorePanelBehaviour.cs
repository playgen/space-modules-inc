using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    private Text _scoreCommentText;

    [SerializeField]
    private Image _emotionImage;

    [SerializeField]
    private Text _emotionText;

    [SerializeField]
    private Sprite _emotionPositiveSprite;

    [SerializeField]
    private Sprite _emotionNegativeSprite;

    [SerializeField]
    private Text _bonusText;

    public void SetScorePanel(int stars, int score, string scoreComment, bool moodImage, string emotionText, int bonus)
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

        _scoreCommentText.text = scoreComment;

        _emotionImage.sprite = moodImage ? _emotionPositiveSprite : _emotionNegativeSprite;

        _emotionText.text = emotionText;

        _bonusText.text = "+ " + bonus.ToString("N0");
    }

}
