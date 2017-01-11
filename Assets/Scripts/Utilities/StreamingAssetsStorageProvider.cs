#if !UNITY_WEBGL || UNITY_EDITOR

using System.IO;
using Assets.Plugins;
using UnityEngine;

public class StreamingAssetsStorageProvider : IStorageProvider
{
	public Stream LoadFile(string absoluteFilePath, FileMode mode, FileAccess access)
	{
		return File.Open(RootPath(absoluteFilePath), mode, access);
	}

	public bool FileExists(string absoluteFilePath)
	{
		return File.Exists(RootPath(absoluteFilePath));
	}

	private static string RootPath(string path)
	{
		if (!Path.IsPathRooted(path))
			//return Application.streamingAssetsPath + path;
			return Path.Combine(Application.streamingAssetsPath, path);
		//return Path.Combine(Application.streamingAssetsPath, path);
		return path;
	}
}

#endif