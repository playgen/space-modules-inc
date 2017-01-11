using System.IO;
using AssetManagerPackage;
using Assets.Scripts;
using GameWork.Unity.Audio;
using UnityEngine;

public class StreamingAssetsAudioChannel : AudioChannel
{
	private AudioClip _clip;

	protected override AudioClip LoadClip(string path)
	{
		var www = new WWW("file://" + path);
		while (!www.isDone)
		{
		}
		
		_clip = www.GetAudioClip(false, false);
		return _clip;
	}
}
