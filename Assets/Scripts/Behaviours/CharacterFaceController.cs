using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class CharacterFaceController : MonoBehaviour
{

    public string CharacterId;    // '<Gender>_Character_<number>', e.g 'Female_Character_01'
    public string Gender;

    [Serializable]
    private class FacialExpression
    {
        public string Name;
        public Sprite Eyebrows;
        public Sprite Eyes;
        public Sprite Mouth;
        public Sprite[] BlinkFrames;
        public Sprite[] MouthFrames;
    }

    [SerializeField]
    [Range(1, 10f)]
    private float _blinkDelay;

    [SerializeField]
    private Image _eyebrowRenderer;

    [SerializeField]
    private Image _eyeRenderer;

    [SerializeField]
    private Image _mouthRenderer;

    [SerializeField]
    private FacialExpression[] _facialExpressions;

    void Start()
    {
        LoadSprites();
        _eyebrowRenderer.enabled = true;
        _eyeRenderer.enabled = true;
        _mouthRenderer.enabled = true;
        SetEmotion("Idle");
    }



    public void SetEmotion(string emotion)
    {
        StopAllCoroutines();
        var expression = _facialExpressions.FirstOrDefault(facialExpression => facialExpression.Name.Equals(emotion));
        if (expression != null)
        {
            _eyebrowRenderer.sprite = expression.Eyebrows;
            _eyeRenderer.sprite = expression.Eyes;

            _mouthRenderer.sprite = expression.Mouth;
            StartCoroutine(IdleAnimation(expression));
        }
    }

    private IEnumerator IdleAnimation(FacialExpression expression)
    {
        while (true)
        {
            _eyeRenderer.sprite = expression.Eyes;

            yield return new WaitForSeconds(_blinkDelay);

            foreach (var sprite in expression.BlinkFrames)
            {
                _eyeRenderer.sprite = sprite;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void LoadSprites()
    {
        var eyebrowSprites = Resources.LoadAll<Sprite>("Sprites/Characters/" + Gender + "_Character_" + CharacterId + "/Eyebrows");
        _facialExpressions = new FacialExpression[eyebrowSprites.Length];
        for (var i = 0; i < eyebrowSprites.Length; i++)
        {
            var substring = "Eyebrows_";
            var index = eyebrowSprites[i].name.IndexOf(substring);
            var emotionName = eyebrowSprites[i].name.Substring(index + substring.Length);
            var mouthId = "02";
            switch (emotionName)
            {
                case "Idle":
                    mouthId = "01";
                    break;
                case "Fear":
                    mouthId = "03";
                    break;
                case "Sadness":
                    mouthId = "03";
                    break;
                case "Joy":
                    mouthId = "04";
                    break;
            }

            var eyeFrames = Resources.LoadAll<Sprite>("Sprites/Characters/" + Gender + "_Base/Eyes/" + emotionName);
            var mouthFrames = Resources.LoadAll<Sprite>("Sprites/Characters/" + Gender + "_Base/Mouth/Mouth_Loop_" + mouthId);
            var expression = new FacialExpression()
            {
                Name = emotionName,
                Eyebrows = eyebrowSprites[i],
                Eyes = eyeFrames[0],
                Mouth = mouthFrames[0],
                BlinkFrames = eyeFrames,
                MouthFrames = mouthFrames
            };
            _facialExpressions[i] = expression;
        }
    }
}
