using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Unity;

using RAGE.EvaluationAsset;

using UnityEngine;
using UnityEngine.UI;

public class LoadingStateInput : TickStateInput
{
	public event Action OfflineClickedEvent;
	public event Action LoggedInEvent;
	private float _timeSinceStart;

	protected override void OnInitialize()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer/OfflineButton")
			.GetComponent<Button>()
			.onClick.AddListener(() =>
			{
				OfflineClickedEvent?.Invoke();
			});
		var backgrounds = GameObjectUtilities.FindGameObject("BackgroundContainer");
		var aspect = Camera.main.aspect;
		var backgroundSize = (3f / 4f) / aspect;
		backgroundSize = backgroundSize < 1 ? 1 : backgroundSize;
		((RectTransform) backgrounds.transform).anchorMin = new Vector2((1 - backgroundSize) * 0.5f, 0);
		((RectTransform) backgrounds.transform).anchorMax = new Vector2(((backgroundSize - 1) * 0.5f) + 1, 1);
	}

	protected override void OnEnter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(true);
		_timeSinceStart = 0;
	}

	protected override void OnTick(float deltaTime) {
		if (_timeSinceStart < 1) {
			_timeSinceStart += deltaTime;
			if (_timeSinceStart >= 1) {
				// Check for SUGAR login
				SUGARManager.Unity.gameObject.GetComponent<AccountUnityClientAdditions>().DisplayPanel(success =>
					{
						if (success)
						{
							var settings = new EvaluationAssetSettings { PlayerId = SUGARManager.CurrentUser.Name };
							EvaluationAsset.Instance.Settings = settings;
							LoggedInEvent?.Invoke();
						}
						else
						{
							LoggedInEvent?.Invoke();
						}
					});
				if (SUGARManager.CurrentUser != null)
				{
					// set username on account panel
					var username = GameObject.Find("SUGAR/SUGAR Canvas/AccountPanel/Username/InputField").GetComponent<InputField>();
					if (username != null)
					{
						username.text = SUGARManager.CurrentUser.Name;
						username.textComponent.text = SUGARManager.CurrentUser.Name;
					}
				}
			}
		}
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanelContainer").SetActive(false);
	}
}