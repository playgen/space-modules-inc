using GameWork.Core.Audio.Clip;
using GameWork.Core.Audio.Fade.Interfaces;
using GameWork.Core.Math;

namespace GameWork.Core.Audio.Fade
{
	public class AudioFade : IAudioFade
	{
	    private readonly AudioFadeModel _model;
	    private float _elapsedTime;

        public AudioClipModel Clip { get; private set; }

        public bool IsComplete
        {
            get { return _model.Duration <= _elapsedTime; }
        }

        public float Volume
        {
            get { return MathF.Lerp(_model.StartVolume, _model.TargetVolume, _elapsedTime / _model.Duration); }
        }

	    public float TargetVolume
	    {
	        get { return _model.TargetVolume; }
	    }

        public float Duration
        {
            get { return _model.Duration; }
        }

        public AudioFade(AudioClipModel clip, float startVolume, float targetVolume, float duration)
		{
			Clip = clip;
		    _model = new AudioFadeModel(startVolume, targetVolume, duration);
		}

        public void AddDeltaTime(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }
    }
}