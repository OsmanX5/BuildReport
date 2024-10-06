using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportNotUsedAssetsVE : VisualElement
{
	private BuildReport report;
	HashSet<string> usedAssets;
	HashSet<string> AllProjectAssets;
	HashSet<string> filtredAssets;
	HashSet<string> notUsedAssets;
	public BuildReportNotUsedAssetsVE(BuildReport report)
	{
		this.report = report;
		buildUsedAssetsSet();
		buildAllProjectAssetsSet();
		FilterAssets();
		notUsedAssets = filtredAssets.Except(usedAssets).ToHashSet();
	}

    private void FilterAssets()
    {
        filtredAssets= AllProjectAssets.
			Where(
				assetPath => FilterAsset(assetPath) 
			).
			OrderByDescending(
				assetPath => new FileInfo(assetPath).Length
				).
			ToHashSet();
		bool FilterAsset(string path)
		{
			if(!path.StartsWith("Assets"))
			{
                return false;
            }
			try
			{
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Length > 10 * 1024)
                {
                    return true;
                }
            }
			catch (Exception e)
			{
                return false;
            }

			return false;
		}
	}

    void buildUsedAssetsSet()
	{
		usedAssets = new HashSet<string>();
		foreach (var scene in report.scenesUsingAssets)
		{
			foreach (var asset in scene.list)
			{
				usedAssets.Add(asset.assetPath);
			}
		}
	}
	void buildAllProjectAssetsSet()
	{
		AllProjectAssets = new HashSet<string>();
		AssetDatabase.GetAllAssetPaths();
		foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
		{
			
			AllProjectAssets.Add(assetPath);
		}
	}
	internal VisualElement GetVE()
	{
		Foldout foldout = new Foldout();
		foldout.text = "Not Used Assets";
		foldout.Add(new AssetsPathsListView(notUsedAssets.ToArray()));
		return foldout;
	}
}
