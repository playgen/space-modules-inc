using GameWork.Core.Audio.PlatformAdaptors;

public class StreamingAssetsAudioChannelFactory : IAudioChannelFactory
{
	public IAudioChannel Create()
	{
		return new StreamingAssetsAudioChannel();
	}
}
