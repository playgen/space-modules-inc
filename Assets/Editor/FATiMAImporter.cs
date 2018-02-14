using System.IO;
using UnityEditor;
using UnityEngine;

public class FATiMAImporter : AssetPostprocessor
{
	public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (asset.EndsWith(".iat") || asset.EndsWith(".ea") || asset.EndsWith(".edm") || asset.EndsWith(".rpc") || asset.EndsWith(".si"))
			{
				string filePath = Application.dataPath + "/Resources/Scenarios/";
				if (!asset.EndsWith(".iat"))
				{
					filePath = Application.dataPath + "/Resources/ScenarioRelated/";
				}
				string newFileName = filePath + Path.GetFileNameWithoutExtension(asset) + ".txt";

				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}

				StreamReader reader = new StreamReader(asset);
				string fileData = reader.ReadToEnd();
				reader.Close();

				FileStream resourceFile = new FileStream(newFileName, FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter writer = new StreamWriter(resourceFile);
				writer.Write(fileData);
				writer.Close();
				resourceFile.Close();

				AssetDatabase.Refresh(ImportAssetOptions.Default);
			}
		}
	}
}
