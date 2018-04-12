using System.IO;

using GameWork.Unity.Engine.Audio;
using UnityEngine;

public class StreamingAssetsAudioChannel : AudioChannel
{
	private AudioClip _clip;

	protected override AudioClip LoadClip(string path)
	{
		var www = new WWW("file://" + Path.Combine(Application.streamingAssetsPath, path + ".ogg"));
		while (!www.isDone)
		{
		}
		
		_clip = www.GetAudioClip(false, false);
		return _clip;
	}
}
