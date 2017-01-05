using GameWork.Core.Audio.Clip;
using GameWork.Core.Audio.PlatformAdaptors;
using UnityEngine;

namespace GameWork.Unity.Audio
{
	public class AudioChannel : IAudioChannel
	{
		private readonly AudioSource _audioSource;
		private IAudioChannel _master;

		public bool IsPlaying
		{
			get { return _audioSource.isPlaying; }
		}

		public float PlaybackSeconds
		{
			get { return _audioSource.time; }
		}

		public int PlaybackSamples
		{
			get { return _audioSource.timeSamples; }
		}

		public float Volume
		{
			get { return _audioSource.volume; }
			set { _audioSource.volume = value; }
		}

		public AudioChannel()
		{
			_audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
		}

		public void Play(AudioClipModel clip, IAudioChannel master = null)
		{
			_master = master;
			// TODO create resource manager and cache resources
			_audioSource.clip = Resources.Load<AudioClip>(clip.Name);
			_audioSource.time = 0f;
			_audioSource.Play();
		}

		public void Stop()
		{
			_audioSource.Stop();
		}

		public void Sync()
		{
			if(_master != null)
			{
				if(_master.IsPlaying && _master.PlaybackSamples < _audioSource.clip.samples)
				{
					_audioSource.timeSamples = (int)_master.PlaybackSamples;
				}
			}
		}
	}
}