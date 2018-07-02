using GameWork.Unity.Engine.Audio;
using UnityEngine;

public class ResourceAudioChannel : AudioChannel
{
	protected override AudioClip LoadClip(string path)
	{
		return Resources.Load<AudioClip>(path);
	}
}