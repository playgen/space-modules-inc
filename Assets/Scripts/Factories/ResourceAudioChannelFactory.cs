using GameWork.Core.Audio.PlatformAdaptors;

public class ResourceAudioChannelFactory : IAudioChannelFactory
{
	public IAudioChannel Create()
	{
		return new ResourceAudioChannel();
	}
}
