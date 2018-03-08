using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;

public class AccountUnityClientAdditions : MonoBehaviour
{
	private Action<bool> _signInCallback;

	internal CommandLineOptions options;

	[Tooltip("Should the application attempt to sign in users using information provided at start-up?")]
	[SerializeField]
	private bool _allowAutoLogin;

	[Tooltip("The default account source token to be used when signing in and registering via the UI object")]
	[SerializeField]
	private string _defaultSourceToken = "SUGAR";

	/// <summary>
	/// Displays UI object if provided and allowAutoLogin is false. Attempts automatic sign in using provided details if allowAutoLogin is true.
	/// Note: allowAutoLogin is made false after automatic sign in is first attempted.
	/// </summary>
	public void DisplayPanel(Action<bool> success)
	{
		_signInCallback = success;

		if (Application.platform == RuntimePlatform.Android && SUGARManager.Client != null)
		{
			SignIn();
		}
		else
		{
			SUGARManager.Account.DisplayPanel(_signInCallback);
		}
	}

	private void SignIn()
	{
		if (_allowAutoLogin)
		{
			var androidParams = AndroidDeepLink.GetURL().Split(new[] { '?' }, 2);
			var androidParam = androidParams.Length != 2 ? string.Empty : androidParams[1] ?? string.Empty;
			var paramArray = androidParam.Split('&');
			var parametersDictionary = new Dictionary<string, string>();
			foreach (var parameter in paramArray)
			{
				var keyValuePair = parameter.Split('=');
				if (keyValuePair.Length == 2)
				{
					parametersDictionary.Add(keyValuePair[0].ToLower(), keyValuePair[1]);
				}
			}
			if (!parametersDictionary.ContainsKey("hash") || !VerifyUrlString(androidParam, parametersDictionary["hash"]))
			{
				SUGARManager.Account.DisplayPanel(_signInCallback);
				return;
			}
			options = new CommandLineOptions();
			CommandLineUtility.CustomArgs = new Dictionary<string, string>();
			foreach (var par in parametersDictionary)
			{
				switch (par.Key.ToLower())
				{
					case "source":
						options.AuthenticationSource = par.Value;
						break;
					case "class":
						options.ClassId = par.Value;
						break;
					case "username":
						options.UserId = par.Value;
						break;
					case "password":
						options.Password = par.Value;
						break;
					case "sessionid":
						CommandLineUtility.CustomArgs.Add("round", par.Value);
						break;
					default:
						CommandLineUtility.CustomArgs.Add(par.Key.ToLower(), par.Value);
						break;
				}
			}
		}
		if (options != null && options.AuthenticationSource == null)
		{
			options.AuthenticationSource = _defaultSourceToken;
		}
		if (options != null && _allowAutoLogin)
		{
			LoginUser(options.UserId, options.Password ?? string.Empty, options.AuthenticationSource);
		}
		else
		{
			SUGARManager.Account.DisplayPanel(_signInCallback);
		}
	}

	internal void LoginUser(string user, string pass, string sourceToken = "")
	{
		if (string.IsNullOrEmpty(sourceToken))
		{
			sourceToken = _defaultSourceToken;
		}
		SUGARManager.Unity.StartSpinner();
		var accountRequest = CreateAccountRequest(user, pass, sourceToken);
		SUGARManager.Client.Session.LoginAsync(SUGARManager.GameId, accountRequest,
		response =>
		{
			SUGARManager.Unity.StopSpinner();
			if (SUGARManager.Unity.GameValidityCheck())
			{
				SUGARManager.CurrentUser = response.User;
				SUGARManager.UserGroup.GetGroupsList(groups => SUGARManager.CurrentGroup = SUGARManager.UserGroup.Groups.FirstOrDefault()?.Actor);
				_allowAutoLogin = false;
				if (options != null) SUGARManager.ClassId = options.ClassId;
				_signInCallback(true);
			}
			SUGARManager.Account.Hide();
		},
		exception =>
		{
			Debug.LogError(exception);
			SUGARManager.Account.DisplayPanel(_signInCallback);
			_signInCallback(false);
			SUGARManager.Unity.StopSpinner();
		});
	}

	public static bool VerifyUrlString(string parameters, string hash)
	{
		var found = parameters.IndexOf("&hash=", StringComparison.Ordinal);
		if (found < 0)
		{
			return false;
		}
		var payload = parameters.Substring(0, found) + "&hash=kYglGuirZwfC2qMi2i0YOtQzyYRoCS7AzGpJHHhSKMFCD26jYqEqK3iPehNQ";
		return ComputeHash(payload) == hash;
	}

	public static string ComputeHash(string input)
	{
		byte[] intputBytes = Encoding.UTF8.GetBytes(input);
		var hashProvider = new SHA1Managed();
		byte[] hashed = hashProvider.ComputeHash(intputBytes);
		string hashValue = BitConverter.ToString(hashed).Replace("-", string.Empty);
		return hashValue;
	}

	private AccountRequest CreateAccountRequest(string user, string pass, string source)
	{
		return new AccountRequest
		{
			Name = user,
			Password = pass,
			SourceToken = source
		};
	}
}
