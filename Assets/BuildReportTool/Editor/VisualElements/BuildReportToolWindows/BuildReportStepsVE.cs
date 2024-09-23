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
using System.Drawing;


/// <summary>
/// ref https://docs.unity3d.com/Manual/UIE-uxml-element-TreeView.html
/// </summary>
public class BuildReportStepsVE : VisualElement
{
	const string templatePath = "Assets/BuildReportTool/Editor/VisualElements/BuildReportToolWindows/BuildReportStepsVE.uxml";
	BuildStepsInfoLogic buildStepsInfoLogic;

	/// <summary>
	/// Query Visual Elements
	/// </summary>
	/// 
	TreeView Steps_TreeView;
	ListView Messeges_ListView;
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
		Messeges_ListView = baseVE.Q<ListView>(nameof(Messeges_ListView));
	}

	/// <summary>
	/// Building the TreeView
	/// </summary>
	List<TreeViewItemData<string>> treeViewItemDatas;
	private void UpdateTreeView()
	{
		var items = buildStepsInfoLogic.CreateTreeViewItemsList();

		Func<VisualElement> makeItem = () => new BuildStepTreeVE();


		Action<VisualElement, int> bindItem = (e, i) =>
		{
			BuildNodeVEDataHolder item = Steps_TreeView.GetItemDataForIndex<BuildNodeVEDataHolder>(i);
			BuildStepTreeVE stepVE = e as BuildStepTreeVE;
			stepVE.SetData(item);
		};

		Steps_TreeView.SetRootItems(items);
		Steps_TreeView.makeItem = makeItem;
		Steps_TreeView.bindItem = bindItem;
		Steps_TreeView.selectionType = SelectionType.Multiple;
		Steps_TreeView.Rebuild();
		Steps_TreeView.itemsChosen += Debug.Log;
		Steps_TreeView.selectedIndicesChanged += OnOnItemPressedInTreeView;
		
	}

	private void OnOnItemPressedInTreeView(IEnumerable<int> enumerable)
	{
		var messegesToShow= new List<BuildStepMessage>();
		foreach (var index in enumerable)
		{
			var selectedItem = Steps_TreeView.GetItemDataForIndex<BuildNodeVEDataHolder>(index);
			foreach (var message in selectedItem.buildStepMessages)
			{
				messegesToShow.Add(message);
			}
			if (selectedItem.childsMesseges.Length> 0)
			{
				foreach (var message in selectedItem.childsMesseges)
					messegesToShow.Add(message);
			}
		}
		UpdateMessegesListView(messegesToShow);
	}
	public void UpdateMessegesListView(List<BuildStepMessage> messegesToShow)
	{
		Messeges_ListView.itemsSource = messegesToShow;
		Messeges_ListView.makeItem = () => new BuildMessegeVE();
		Messeges_ListView.bindItem = (e, i) =>
		{
			try
			{
				BuildMessegeVE item = e as BuildMessegeVE;
				item.SetData(messegesToShow[i]);
			}
			catch (Exception ex)
			{
				//Debug.LogWarning(ex);
			}
		};
		Messeges_ListView.Rebuild();
	}
	public void RegisterEvents()
	{
	}


}
public class BuildMessegeVE : VisualElement
{
	public BuildMessegeVE()
	{
	}

	public void SetData(BuildStepMessage buildStepMessage)
	{
		Clear();
		VisualElement ve = new VisualElement();
		ve.Clear();
		ve.style.flexDirection = FlexDirection.Row;
		ve.style.marginTop = 5;
		ve.style.marginBottom = 5;

		Texture2D Icon;
		switch (buildStepMessage.type)
		{
			case LogType.Error:
				Icon = IconsLibrary.Instance.Core.GetIcon("Error");
				ve.style.backgroundColor = new UnityEngine.Color(1, 0, 0, 0.1f);
				break;
			case LogType.Warning:
				Icon = IconsLibrary.Instance.Core.GetIcon("Warning");
				ve.style.backgroundColor = new UnityEngine.Color(1, 1, 0, 0.1f);
				break;
			default:
				Icon = IconsLibrary.Instance.Core.GetIcon("Info");
				break;
		}
		VisualElement IconBox = new VisualElement();
		IconBox.style.backgroundImage = Icon;
		IconBox.style.width = 24;
		IconBox.style.height = 24;
		IconBox.style.flexShrink = 0;
		ve.Add(IconBox);

		Label messegeTextLabel = new Label();
		messegeTextLabel.text = buildStepMessage.content;
		messegeTextLabel.style.flexGrow = 1;
		messegeTextLabel.style.whiteSpace = WhiteSpace.Normal;

		ve.style.alignItems = Align.Center;
		ve.Add(messegeTextLabel);
		Add(ve);
	}
}
public class BuildStepTreeVE : VisualElement
{
	VisualElement ve;
	public BuildStepTreeVE()
	{
		ve = new VisualElement();
		Add(ve);
	}
	public void SetData(BuildNodeVEDataHolder node)
	{
		ve.Clear();
		ve.style.flexDirection = FlexDirection.Row;
		ve.style.marginLeft = 5;
		ve.style.marginRight = 5;


		VisualElement colorBox = new VisualElement();
		colorBox.style.backgroundImage= node.Icon;
		colorBox.style.width = 16;
		colorBox.style.height = 16;
		colorBox.style.flexShrink = 1;
		ve.Add(colorBox);

		Label TypeNameLabel = new Label();
		TypeNameLabel.text = node.StepName;
		TypeNameLabel.style.flexGrow = 1;

		ve.style.alignItems = Align.Center;
		ve.Add(TypeNameLabel);

	}
}