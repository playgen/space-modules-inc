using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CallAnimationBehaviour : MonoBehaviour
{
	[SerializeField]
	private Image[] _callRings;

	[SerializeField]
	private Sprite[] _ringSprites;

	[SerializeField]
	private Sprite _emptySprite;

	[SerializeField]
	private float _animationDelay = 0.1f;

	private bool _ringing;

	public void StartAnimation()
	{
		if (!_ringing)
		{
			StartCoroutine(RingLoop());
		}
	}

	public void StopAnimation()
	{
		_ringing = false;
	}

	private IEnumerator RingLoop()
	{
		_ringing = true;
		var delay = new WaitForSeconds(_animationDelay);
		var frameCounter = 0;
		var maxFrameCount = _ringSprites.Length + _callRings.Length;

		while (_ringing)
		{
			yield return delay;
			for (var i = 0; i < _callRings.Length; i++)
			{
				var index = frameCounter - i;
				if (index < _ringSprites.Length && index >= 0)
				{
					_callRings[i].sprite = _ringSprites[index];
				}
				else
				{
					_callRings[i].sprite = _emptySprite;
				}
			}
			frameCounter++;
			if (frameCounter >= maxFrameCount)
			{
				frameCounter = 0;
			}
		}
	}
}
