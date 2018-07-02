using System;
using System.IO;
using System.Net;
using System.Text;

using AssetPackage;

using UnityEngine;

public class AssetManagerBridge : IBridge, ILog, IDataStorage, IWebServiceRequest
{
	public void Log(Severity severity, string msg)
	{
		switch (severity)
		{
			case Severity.Critical:
			case Severity.Error:
				Debug.LogError(msg);
				break;
			case Severity.Warning:
				Debug.LogWarning(msg);
				break;
			case Severity.Information:
			case Severity.Verbose:
				//Debug.Log(msg);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
		}
	}

	public bool Delete(string fileId)
	{
		throw new InvalidOperationException();
	}

	public bool Exists(string fileId)
	{
		fileId = FilePathFormat(fileId);
		var loaded = Resources.Load(fileId);
		return loaded;
	}

	public string[] Files()
	{
		throw new InvalidOperationException();
	}

	public string Load(string fileId)
	{
		fileId = FilePathFormat(fileId);
		var fileText = Resources.Load<TextAsset>(fileId)?.text;
		if (!string.IsNullOrEmpty(fileText))
		{
			fileText = fileText.Replace("..\\\\", "..\\\\..\\\\");
			fileText = fileText.Replace("..//", "..//..//");
		}
		return fileText;
	}

	public void Save(string fileId, string fileData)
	{
		using (var writer = File.CreateText(fileId))
		{
			writer.Write(fileData);
		}
	}

	private string FilePathFormat(string fileId)
	{
		if (fileId.StartsWith("\\") || fileId.StartsWith("/"))
		{
			fileId = fileId.Remove(0, 1);
		}
		fileId = fileId.Replace(".rpc", string.Empty);
		fileId = fileId.Replace(".ea", string.Empty);
		fileId = fileId.Replace(".edm", string.Empty);
		fileId = fileId.Replace(".si", string.Empty);
		return fileId;
	}

	public void Append(string fileId, string fileData)
	{
		throw new NotImplementedException();
	}

	public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
	{
		var result = new RequestResponse(requestSettings);

		try
		{
			var request = (HttpWebRequest)WebRequest.Create(requestSettings.uri);
			var stream = request.GetRequestStream();
			request.Method = requestSettings.method;
			if (requestSettings.requestHeaders.ContainsKey("Accept"))
			{
				request.Accept = requestSettings.requestHeaders["Accept"];
			}
			if (!string.IsNullOrEmpty(requestSettings.body))
			{
				var data = Encoding.UTF8.GetBytes(requestSettings.body);
				if (requestSettings.requestHeaders.ContainsKey("Content-Type"))
				{
					request.ContentType = requestSettings.requestHeaders["Content-Type"];
				}
				foreach (var kvp in requestSettings.requestHeaders)
				{
					if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
					{
						continue;
					}
					request.Headers.Add(kvp.Key, kvp.Value);
				}
				request.ContentLength = data.Length;
				request.ServicePoint.Expect100Continue = false;
				stream.Write(data, 0, data.Length);
				stream.Close();
			}
			else
			{
				foreach (var kvp in requestSettings.requestHeaders)
				{
					if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
					{
						continue;
					}
					request.Headers.Add(kvp.Key, kvp.Value);
				}
			}

			var response = request.GetResponse();
			if (response.Headers.HasKeys())
			{
				foreach (var key in response.Headers.AllKeys)
				{
					result.responseHeaders.Add(key, response.Headers.Get(key));
				}
			}
			result.responseCode = (int)((HttpWebResponse)response).StatusCode;
			using (var reader = new StreamReader(stream))
			{
				result.body = reader.ReadToEnd();
			}
		}
		catch (Exception e)
		{
			result.responsMessage = e.Message;

			Log(Severity.Error, string.Format("{0} - {1}", e.GetType().Name, e.Message));
		}

		requestResponse = result;
	}
}