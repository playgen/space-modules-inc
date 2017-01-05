using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Controllers;
using GameWork.Core.Audio.Clip;
using GameWork.Core.Audio.Fade;
using GameWork.Core.Audio.Fade.Interfaces;
using GameWork.Core.Audio.PlatformAdaptors;

namespace GameWork.Core.Audio
{
	public class AudioController : Controller
	{
        private readonly float _volumeLowerLimit;
        private readonly float _volumeUpperLimit;

		private readonly IAudioChannel[] _channels;
        private readonly List<IAudioFade> _activeAudioFades;
        private readonly Dictionary<AudioClipModel, IAudioChannel> _occupiedChannels = new Dictionary<AudioClipModel, IAudioChannel>();

	    public AudioController(IAudioChannelFactory audioChannelFactory, int channelCount = 32, float volumeLowerLimit = 0, float volumeUpperLimit = 1)
	    {
	        _volumeLowerLimit = volumeLowerLimit;
	        _volumeUpperLimit = volumeUpperLimit;

            _activeAudioFades = new List<IAudioFade>(channelCount);
            _channels = InitializeChannels(audioChannelFactory, channelCount);
        }

	    public override void Tick(float deltaTime)
		{
			ProcessFades(deltaTime);
			SyncSlaves();
		}

		public bool IsPlaying(AudioClipModel clip)
		{
			var isPlaying = false;

			if(_occupiedChannels.ContainsKey(clip))
			{
				isPlaying = _occupiedChannels[clip].IsPlaying;
			}

			return isPlaying;
		}

	    public bool IsFading(AudioClipModel clip, float? targetVolume, float? duration)
	    {
	        var isFading = false;

	        foreach (var fade in _activeAudioFades)
	        {
	            if (fade.Clip == clip
                    && (targetVolume == null || fade.TargetVolume == targetVolume)
	                && (duration == null || fade.Duration == duration))
	            {
	                isFading = true;
	                break;
	            }
	        }

	        return isFading;
	    }

	    public bool TryGetVolume(AudioClipModel clip, out float volume)
		{
			var success = false;
			volume = 0f;

			if (_occupiedChannels.ContainsKey(clip))
			{
				volume = _occupiedChannels[clip].Volume;
				success = true;
			}

			return success;
		}

		public float GetPlaybackTime(AudioClipModel clip)
		{
			var playbackTime = -1f;
			IAudioChannel channel;

			if(TryGetChannel(clip, out channel))
			{
				playbackTime = channel.PlaybackSeconds;
			}

			return playbackTime;
		}

		public void Play(AudioClipModel clip, AudioClipModel master = null)
		{
			PlayInternal(clip, master);
		}

		public void Stop(AudioClipModel clip)
		{
			IAudioChannel channel;

			if(TryGetChannel(clip, out channel))
			{
				ResetChannel(channel);
				_occupiedChannels.Remove(clip);
			}

			TryRemoveFade(clip);
		}

		public void FadeIn(AudioClipModel clip, float duration, AudioClipModel master = null)
		{
			var freeChannel = PlayInternal(clip, master);

			if(0f < duration)
			{
				if(freeChannel != null)
				{
					freeChannel.Volume = _volumeLowerLimit;
					AddFade(clip, _volumeLowerLimit, _volumeUpperLimit, duration);
				}
			}
		}

		public void FadeOut(AudioClipModel clip, float duration)
		{
			if(duration <= 0f)
			{
				Stop(clip);
			}
			else
			{
				IAudioChannel channel;

				if(TryGetChannel(clip, out channel))
				{
					var startVolume = channel.Volume;
					AddFade(clip, startVolume, _volumeLowerLimit, duration);
				}
			}
		}

		public void Fade(AudioClipModel clip, float targetVolume, float duration)
		{
			IAudioChannel channel;

			if (TryGetChannel(clip, out channel))
			{
				AddFade(clip, channel.Volume, targetVolume, duration);
			}
		}

        public void Fade(AudioClipModel clip, params AudioFadeModel[] audioFades)
        {
            TryRemoveFade(clip);
            var fade = new AudioMultiFade(clip, audioFades);
            _activeAudioFades.Add(fade);
        }

        private void AddFade(AudioClipModel clip, float startVolume, float endVolume, float duration)
		{
			TryRemoveFade(clip);
			var fade = new AudioFade(clip, startVolume, endVolume, duration);
			_activeAudioFades.Add(fade);
		}

        private bool TryGetChannel(AudioClipModel clip, out IAudioChannel channel)
		{
			var didFindChannel = false;
			channel = null;

			if(_occupiedChannels.ContainsKey(clip))
			{
				channel = _occupiedChannels[clip];
				didFindChannel = true;
			}
			else
			{
				// TODO use a global logger
				// Debug.LogWarning("No channel found for clip: " + clip.Name);
			}

			return didFindChannel;
		}

		private IAudioChannel PlayInternal(AudioClipModel clip, AudioClipModel master = null)
		{
			IAudioChannel channel;

			if(IsPlaying(clip))
			{
				channel = _occupiedChannels[clip];
				TryRemoveFade(clip);
			}
			else if(TryFindFreeChannel(out channel))
			{
				_occupiedChannels[clip] = channel;
			}

			if(channel != null)
			{
				if(master != null)
				{
					IAudioChannel masterChannel;
					TryGetChannel(master, out masterChannel);
					channel.Play(clip, masterChannel);
				}
				else
				{
					channel.Play(clip);
				}
			}

			return channel;
		}

		private bool TryRemoveFade(AudioClipModel clip)
		{
			var didStopFade = false;

			for(var i = 0; i < _activeAudioFades.Count; i++)
			{
				if(_activeAudioFades[i].Clip == clip)
				{
					_activeAudioFades.RemoveAt(i);
					didStopFade = true;
					break;
				}
			}

			return didStopFade;
		}

		private void ResetChannel(IAudioChannel channel)
		{
			channel.Stop();
			channel.Volume = _volumeUpperLimit;
		}

		private void SyncSlaves()
		{
		    foreach (var channel in _channels)
		    {
		        if(channel.IsPlaying)
		        {
		            channel.Sync();
		        }
		    }
		}

        private bool TryFindFreeChannel(out IAudioChannel freeChannel)
        {
            freeChannel = _channels.FirstOrDefault(c => !c.IsPlaying);
            return freeChannel != null;
        }

        private void ProcessFades(float deltaTime)
		{
			for(var i = _activeAudioFades.Count - 1; 0 <= i; i--)
			{
				var fade = _activeAudioFades[i];

				fade.AddDeltaTime(deltaTime);

				var channel = _occupiedChannels[fade.Clip];
				channel.Volume = fade.Volume;
					
				if(fade.IsComplete)
				{
					_activeAudioFades.Remove(fade);

					if(channel.Volume == 0f)
					{
						Stop(fade.Clip);
					}										
				}
			}
		}

        private IAudioChannel[] InitializeChannels(IAudioChannelFactory factory, int channelCount)
	    {
            var channels = new IAudioChannel[channelCount];

	        for (var i = 0; i < channelCount; i++) 
	        {
	            channels[i] = factory.Create();
	        }

	        return channels;
	    }
	}
}