using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.PlayerLoop;
using System;

/// <summary>
/// ref https://docs.unity3d.com/Manual/UIE-uxml-element-TreeView.html
/// </summary>
public class BuildReportStepsVE : VisualElement
{
	const string templatePath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/BuildReportStepsVE.uxml";
	//const string packedAssetInfoItemVEpath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/PackedAssetInfoItemVE.uxml";
	BuildStepsInfoLogic buildStepsInfoLogic;

	/// <summary>
	/// Query Visual Elements
	/// </summary>
	/// 
	TreeView Steps_TreeView;
	public BuildReportStepsVE(BuildStep[] buildSteps)
	{
		buildStepsInfoLogic = new BuildStepsInfoLogic(buildSteps);
	}

	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		RegisterEvents();
		UpdateTreeView();
		return result.Children().ToList()[0];
	}
	public void QueryVisualElements(VisualElement baseVE)
	{
		Steps_TreeView = baseVE.Q<TreeView>(nameof(Steps_TreeView));
	}
	private void UpdateTreeView()
	{
		var nodes = buildStepsInfoLogic.GetBuildStepNodes();
		List<TreeViewItemData<string>> treeViewItemDatas = new();
		Debug.Log($"nodes count :{nodes.Count}");
		for (int i = 0; i < nodes.Count; i++)
		{
			List<TreeViewItemData<string>> childs = new();
			treeViewItemDatas.Add(new TreeViewItemData<string>(i, nodes[i], childs));
		}


		void BindTreeViewItem(VisualElement element, int i)
		{
			Label label = element as Label;
			label.text = Steps_TreeView.GetItemDataForIndex<string>(i);
		}

		VisualElement TreeViewItemCreator()
		{
			return new Label();
		}

		Steps_TreeView.Clear();
		Steps_TreeView.SetRootItems(treeViewItemDatas);
		Steps_TreeView.makeItem = TreeViewItemCreator;
		Steps_TreeView.bindItem = BindTreeViewItem;
		Steps_TreeView.Rebuild();

		Steps_TreeView.itemsChosen += Steps_TreeView_itemsChosen;
	}

	private void Steps_TreeView_itemsChosen(IEnumerable<object> obj)
	{
		Debug.Log("Steps_TreeView_itemsChosen");
	}

	public void RegisterEvents()
	{
	}


}
