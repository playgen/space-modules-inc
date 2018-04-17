using GameWork.Unity.Engine.Audio;
using UnityEngine;

public class ResourceAudioChannel : AudioChannel
{
	private AudioClip _clip;

	protected override AudioClip LoadClip(string path)
	{
		_clip = Resources.Load<AudioClip>(path);
		return _clip;
	}
}
