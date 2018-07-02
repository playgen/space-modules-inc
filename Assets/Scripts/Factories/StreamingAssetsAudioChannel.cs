using System.IO;

using GameWork.Unity.Engine.Audio;
using UnityEngine;

public class StreamingAssetsAudioChannel : AudioChannel
{
	protected override AudioClip LoadClip(string path)
	{
		var www = new WWW("file://" + Path.Combine(Application.streamingAssetsPath, path + ".ogg"));
		while (!www.isDone)
		{
		}

		return www.GetAudioClip(false, false);
	}
}