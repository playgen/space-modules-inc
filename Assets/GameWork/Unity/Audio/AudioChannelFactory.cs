using GameWork.Core.Audio.PlatformAdaptors;

namespace GameWork.Unity.Audio
{
    public class AudioChannelFactory : IAudioChannelFactory
    {
        public IAudioChannel Create()
        {
            return new AudioChannel();
        }
    }
}