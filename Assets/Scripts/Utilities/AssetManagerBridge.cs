using System;
using System.IO;
using AssetPackage;

using UnityEngine;


public class AssetManagerBridge : IBridge, ILog, IDataStorage
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
				Debug.Log(msg);
				break;
			default:
				throw new ArgumentOutOfRangeException("severity", severity, null);
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
}
