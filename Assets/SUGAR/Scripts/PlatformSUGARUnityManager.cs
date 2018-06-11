using System.Collections;
using System.Collections.Generic;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Unity;

public class PlatformSUGARUnityManager : SUGARUnityManager
{
    protected override IHttpHandler CreateHttpHandler()
    {
#if UNITY_WEBGL
        return new UnityWebGlHttpHandler();
#else
        return null;
#endif
    }
}