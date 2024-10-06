using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportScenesRefrance : VisualElement
{
	private BuildReport report;
	Dictionary<string, HashSet<string>> reportScenes; // Scene Path, Assets Paths
	Dictionary<string, ulong> scenesSizes;
	public BuildReportScenesRefrance(BuildReport report)
	{
		this.report = report;
		CreateReportScenesDict();
	}
	void CreateReportScenesDict()
	{
		reportScenes = new Dictionary<string, HashSet<string>>();
        scenesSizes = new Dictionary<string, ulong>();

        foreach (var scene in report.scenesUsingAssets)
		{
			foreach(var asset in scene.list)
			{
				if (!asset.assetPath.StartsWith("Assets"))
				{
					continue;
				}
				foreach(var scenePath in asset.scenePaths)
				{
					if (!reportScenes.ContainsKey(scenePath))
					{
						reportScenes.Add(scenePath, new HashSet<string>());
                        scenesSizes.Add(scenePath, 0);
                    }
					reportScenes[scenePath].Add(asset.assetPath);
					var assetSize = (new FileInfo(asset.assetPath)).Length;
					scenesSizes[scenePath] += (ulong)assetSize;
				}
			}
		}
	}
	internal VisualElement GetVE()
	{
		VisualElement visualElement = new VisualElement();
		foreach (var scenePath in reportScenes.Keys)
		{
			visualElement.Add(SceneUsageFoldout(scenePath));
		}
		return visualElement;
	}
	VisualElement SceneUsageFoldout(string scenePath)
	{
		var foldout = new Foldout();
		foldout.text = $"{Path.GetFileNameWithoutExtension(scenePath)} [{scenesSizes[scenePath].FormatSize()}]";
		var assetsPaths =  reportScenes[scenePath];
		var assetsPathsListView = new AssetsPathsListView(assetsPaths.ToArray());
		foldout.style.maxHeight = 300;
		foldout.Add(assetsPathsListView);
		foldout.value = false;
		return foldout;
	}
}
