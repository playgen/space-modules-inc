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

	private FacialExpression _currentExpression;
	private bool _talkingAnimation;
	private bool _idleAnimation;

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
		_currentExpression = _facialExpressions.FirstOrDefault(facialExpression => facialExpression.Name.Equals(emotion));
		if (_currentExpression != null)
		{
			_eyebrowRenderer.sprite = _currentExpression.Eyebrows;
			_eyeRenderer.sprite = _currentExpression.Eyes;

			_mouthRenderer.sprite = _currentExpression.Mouth;
			if (!_idleAnimation)
			{
				StartCoroutine(IdleAnimation());
			}

		}
	}

	private IEnumerator IdleAnimation()
	{
		_idleAnimation = true;
		while (_idleAnimation)
		{
			_eyeRenderer.sprite = _currentExpression.Eyes;

			yield return new WaitForSeconds(_blinkDelay);

			foreach (var sprite in _currentExpression.BlinkFrames)
			{
				_eyeRenderer.sprite = sprite;
				yield return new WaitForFixedUpdate();
			}
		}
	}

	public void StartTalkAnimation()
	{
		if (!_talkingAnimation)
		{
			StartCoroutine(TalkAnimation());
		}
	}

	public void StopTalkAnimation()
	{
		_talkingAnimation = false;
	}

	private IEnumerator TalkAnimation()
	{
		_talkingAnimation = true;
		while (_talkingAnimation)
		{
			_mouthRenderer.sprite = _currentExpression.Mouth;

			yield return new WaitForSeconds(0.2f);

			foreach (var sprite in _currentExpression.MouthFrames)
			{
				_mouthRenderer.sprite = sprite;
				yield return new WaitForSeconds(0.01f);
			}
		}
		_mouthRenderer.sprite = _currentExpression.Mouth;
	}

	private void LoadSprites()
	{
		var eyebrowSprites = Resources.LoadAll<Sprite>("Sprites/Characters/" + Gender + "_Character_" + CharacterId + "/Eyebrows");
		_facialExpressions = new FacialExpression[eyebrowSprites.Length];
		for (var i = 0; i < eyebrowSprites.Length; i++)
		{
			var substring = "Eyebrows_";
			var index = eyebrowSprites[i].name.IndexOf(substring, StringComparison.Ordinal);
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
