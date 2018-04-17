using System.IO;
using UnityEditor;
using UnityEngine;

public class FATiMAImporter : AssetPostprocessor
{
	public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (var asset in importedAssets)
		{
			if (asset.EndsWith(".iat") || asset.EndsWith(".ea") || asset.EndsWith(".edm") || asset.EndsWith(".rpc") || asset.EndsWith(".si"))
			{
				var filePath = Application.dataPath + "/Resources/Scenarios/";
				if (!asset.EndsWith(".iat"))
				{
					filePath = Application.dataPath + "/Resources/ScenarioRelated/";
				}
				var newFileName = filePath + Path.GetFileNameWithoutExtension(asset) + ".txt";

				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}

				var reader = new StreamReader(asset);
				var fileData = reader.ReadToEnd();
				reader.Close();

				var resourceFile = new FileStream(newFileName, FileMode.OpenOrCreate, FileAccess.Write);
				var writer = new StreamWriter(resourceFile);
				writer.Write(fileData);
				writer.Close();
				resourceFile.Close();

				AssetDatabase.Refresh(ImportAssetOptions.Default);
			}
		}
	}
}
