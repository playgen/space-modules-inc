using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Unity;

public class PlatformSUGARUnityManager : SUGARUnityManager
{
	protected override SUGARClient CreateSUGARClient(string baseAddress)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return new SUGARClient(baseAddress, new UnityWebGlHttpHandler(), false);
#else
		return base.CreateSUGARClient(baseAddress);
#endif
	}
}