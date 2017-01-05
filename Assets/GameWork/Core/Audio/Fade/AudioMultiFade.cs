using System;
using GameWork.Core.Audio.Fade.Interfaces;
using GameWork.Core.Math;
using GameWork.Core.Audio.Clip;

namespace GameWork.Core.Audio.Fade
{
	public class AudioMultiFade : IAudioFade
	{
	    private readonly AudioFadeModel[] _audioFadeModels;
        private AudioFadeModel _currentModel;
	    private float _completedDurations;
	    private float _elapsedTime;

        public AudioClipModel Clip { get; private set; }

        public bool IsComplete
        {
            get { return _completedDurations + _currentModel.Duration <= _elapsedTime; }
        }

        public float Volume
        {
            get
            {
                return MathF.Lerp(_currentModel.StartVolume,
                 _currentModel.TargetVolume,
              (_elapsedTime - _completedDurations) / _currentModel.Duration);
            }
        }

        public float TargetVolume
        {
            get { return _currentModel.TargetVolume; }
        }

        public float Duration
        {
            get { return _currentModel.Duration; }
        }

        public AudioMultiFade(AudioClipModel clip, params AudioFadeModel[] audioFadeModels)
		{
			Clip = clip;

		    _audioFadeModels = audioFadeModels;
            _currentModel = audioFadeModels[0];
        }

		public void AddDeltaTime(float deltaTime)
        {
            _elapsedTime += deltaTime;

	        var nextModelIndex = Array.IndexOf(_audioFadeModels, _currentModel) + 1;

            while (_completedDurations + _currentModel.Duration < _elapsedTime)
            {
                if (nextModelIndex < _audioFadeModels.Length)
                {
                    _completedDurations += _currentModel.Duration;
                    _currentModel = _audioFadeModels[nextModelIndex];
                    nextModelIndex++;
                }
                else
                {
                    break;
                }
            }
	    }	
	}
}