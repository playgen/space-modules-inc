using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CallAnimationBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _callRings;

    [SerializeField]
    private Sprite[] _ringSprites;

    [SerializeField]
    private Sprite _emptySprite;

    [SerializeField]
    private float _animationDelay = 0.1f;

    private Image[] _ringImages;

    private int _frameCounter;

    void Awake()
    {
        _ringImages = new Image[_callRings.Length];

        for (var i = 0; i < _callRings.Length; i++)
        {
            _ringImages[i] = _callRings[i].GetComponent<Image>();
        }
    }

    public void StartAnimation()
    {
        StartCoroutine(RingLoop());
    }

    public void StopAnimation()
    {
        StopCoroutine(RingLoop());
    }

    IEnumerator RingLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_animationDelay);
            for (var i = 0; i < _ringImages.Length; i++)
            {
                var index = _frameCounter - i;
                if (index < _ringSprites.Length && index >= 0)
                {
                    _ringImages[i].sprite = _ringSprites[index];
                }
                else
                {
                    _ringImages[i].sprite = _emptySprite;
                }
            }
            _frameCounter++;
            if (_frameCounter >= 8)
            {
                _frameCounter = 0;
            }
        }
    }
}
