using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CharacterFaceController : MonoBehaviour
{
	public string CharacterId;    // '<Gender>_Character_<number>', e.g 'Female_Character_01'
	public string Gender;

	[Serializable]
	private struct FacialExpression
	{
		public string Name;
		public Sprite Eyebrows;
		public Sprite Eyes;
		public Sprite Mouth;
		public Sprite[] BlinkFrames;
		public Sprite[] MouthFrames;
	}

	private const float Min = 1f;
	private const float Max = 10f;

	[SerializeField]
	private Image _eyebrowRenderer;

	[SerializeField]
	private Image _eyeRenderer;

	[SerializeField]
	private Image _mouthRenderer;

	[SerializeField]
	private FacialExpression[] _facialExpressions;
	private Dictionary<string, FacialExpression> _facialExpressionDictionary;

	private FacialExpression _currentExpression;
	private bool _talkingAnimation;
	private bool _idleAnimation;

	private void Start()
	{
		_facialExpressionDictionary = _facialExpressions.ToDictionary(f => f.Name, f => f);
		SetEmotion("Idle");
		_eyebrowRenderer.enabled = true;
		_eyeRenderer.enabled = true;
		_mouthRenderer.enabled = true;
	}

	public void SetEmotion(string emotion)
	{
		if (_facialExpressionDictionary.TryGetValue(emotion, out _currentExpression))
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
		var fixedDelay = new WaitForFixedUpdate();

		while (_idleAnimation)
		{
			_eyeRenderer.sprite = _currentExpression.Eyes;
			var blinkDelay = UnityEngine.Random.Range(Min, Max);
			yield return new WaitForSeconds(blinkDelay);
			foreach (var sprite in _currentExpression.BlinkFrames)
			{
				_eyeRenderer.sprite = sprite;
				yield return fixedDelay;
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
		var audioSource = FindObjectOfType<AudioSource>();

		_mouthRenderer.sprite = _currentExpression.Mouth;
		while (_talkingAnimation)
		{
			foreach (var sprite in _currentExpression.MouthFrames)
			{
				_mouthRenderer.sprite = sprite;
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.035f, 0.06f));
			}
			if (!audioSource.isPlaying)
			{
				_talkingAnimation = false;
				audioSource.timeSamples = audioSource.clip.samples;
			}
		}
		_mouthRenderer.sprite = _currentExpression.Mouth;
	}
}
