using GameWork.Core.Controllers.Interfaces;

namespace GameWork.Core.Audio.Interfaces
{
	public interface IAudioController : IController
	{
		float GetPlaybackTime(IAudioClip clip);

		bool IsPlaying(IAudioClip clip);

		void Play(IAudioClip clip, IAudioClip master = null);

		void Stop(IAudioClip clip);

		void FadeIn(IAudioClip clip, float duration, IAudioClip master = null);

		void FadeOut(IAudioClip clip, float duration);
	}
}