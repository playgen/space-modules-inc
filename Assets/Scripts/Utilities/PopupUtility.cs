using System;

public static class PopupUtility
{
    public static event Action<string> LogErrorEvent;
    public static event Action StartLoadingEvent;
    public static event Action EndLoadingEvent;
    public static void LogError(string message)
	{
		LogErrorEvent?.Invoke(message);
	}
    public static void ShowLoadingPopup()
	{
		StartLoadingEvent?.Invoke();
	}

    public static void HideLoadingPopup()
	{
		EndLoadingEvent?.Invoke();
	}
}
