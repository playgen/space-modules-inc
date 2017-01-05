using GameWork.Core.Audio.Clip;

namespace GameWork.Core.Audio.Fade.Interfaces
{
	public interface IAudioFade
	{
	    AudioClipModel Clip { get; }

	    bool IsComplete { get; }

		float Volume { get; }

        float TargetVolume { get; }

        float Duration { get; }

	    void AddDeltaTime(float deltaTime);
	}
}