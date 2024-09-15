using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportPackedAssetsVE : VisualElement
{
	const string templatePath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/BuildReportPackedAssetsVE.uxml";
	const string packedAssetInfoItemVEpath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/PackedAssetInfoItemVE.uxml";
	PackedAssetInfo[] allAssetsInfos;
	ListView packedAssets_ListView;
	public BuildReportPackedAssetsVE(PackedAssets[] packedAssets)
	{
		allAssetsInfos = packedAssets.SelectMany(x => x.contents).OrderBy(info => 0 - info.packedSize).ToArray();
	}
	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		SetPackedAssetsInfo(allAssetsInfos);
		return result.Children().ToList()[0];
	}
	public void QueryVisualElements(VisualElement baseVE)
	{
		packedAssets_ListView = baseVE.Q<ListView>(nameof(packedAssets_ListView));
		if (packedAssets_ListView == null) {
			Debug.Log("packedAssets_ListView is null");
		}
	}
	void SetPackedAssetsInfo(PackedAssetInfo[] assetsInfos)
	{
		packedAssets_ListView.itemsSource = assetsInfos;
		packedAssets_ListView.makeItem = packedAssetListViewItemCreator;
		packedAssets_ListView.bindItem = packedAssetListViewItemBinder;
	}

	private void packedAssetListViewItemBinder(VisualElement element, int index)
	{
		PackedAssetInfo info = (PackedAssetInfo)packedAssets_ListView.itemsSource[index];
		element.style.height = 42;
		Label SizeVE = element.Q<Label>("Size");
		SizeVE.text = info.packedSize.FormatSize();

		Label NameVE = element.Q<Label>("Name");
		NameVE.text = Path.GetFileName(info.sourceAssetPath);
		VisualElement IconVE = element.Q<VisualElement>("Icon");
		Texture2D icon = IconsLibrary.Instance.Core.GetIcon("Unknown");
		if (info.type == typeof(UnityEngine.Texture2D) || info.type == typeof(Sprite))
			icon = IconsLibrary.Instance.Types.GetIcon("Texture");
		else if (info.type == typeof(MonoScript))
			icon = IconsLibrary.Instance.Types.GetIcon("Script");
		else if (info.type == typeof(Material))
			icon = IconsLibrary.Instance.Types.GetIcon("Material");
		else if (info.type == typeof(Shader))
			icon = IconsLibrary.Instance.Types.GetIcon("Shader");
		else
			Debug.Log(info.type.Name);
		IconVE.style.backgroundImage = icon;
	}

	private VisualElement packedAssetListViewItemCreator()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(packedAssetInfoItemVEpath);
		if(visualTree == null)
		{
			Debug.Log("packedAssetInfoItemVEpath is null");
		}
		VisualElement temp = visualTree.CloneTree();
		return temp;
	}


}
