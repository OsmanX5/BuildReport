using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.UIElements;

public class BuildReportDeepContentVE
{
	const string templatePath = "Assets/BuildReportTool/Editor/VisualElements/BuildReportToolWindows/BuildReportDeepContentVE.uxml";
	BuildReport report;
	ContentType selectedContetType;


	Toolbar ContentType_Toolbar;
	VisualElement Content;

	ToolbarButton BuildSteps_ToolbarBtn;
	VisualElement BuildSteps_Content;

	ToolbarButton PackedAssets_ToolbarBtn;
	VisualElement PackedAssets_Content;

	Dictionary<ToolbarButton , VisualElement> toolbarButtonsContentDict;
	public BuildReportDeepContentVE(BuildReport report)
	{
		this.report = report;
		toolbarButtonsContentDict = new Dictionary<ToolbarButton, VisualElement>();
		//toolbarButtonsContentDict.Add(BuildSteps_ToolbarBtn, BuildSteps_Content);
		//toolbarButtonsContentDict.Add(PackedAssets_ToolbarBtn, PackedAssets_Content);
	}

	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		RegisterEvents();
		return result.Children().ToList()[0];
	}
	public void QueryVisualElements(VisualElement baseVE)
	{
		ContentType_Toolbar = baseVE.Q<Toolbar>(nameof(ContentType_Toolbar));
		BuildSteps_ToolbarBtn = ContentType_Toolbar.Q<ToolbarButton>(nameof(BuildSteps_ToolbarBtn));
		PackedAssets_ToolbarBtn = ContentType_Toolbar.Q<ToolbarButton>(nameof(PackedAssets_ToolbarBtn));
		Content = baseVE.Q<VisualElement>(nameof(Content));
	}
	void UpdateContentVE()
	{
		Content.Clear();
		switch (selectedContetType)
		{
			case ContentType.BuildSteps:
				//BuildSteps_Content = new BuildReportBuildStepsVE(report.steps).GetVE();
				VisualElement buildStepsVE = new BuildReportStepsVE(report.steps).GetVE();
				Content.Add(buildStepsVE);
				break;
			case ContentType.PackedAssets:
				BuildReportPackedAssetsVE packedAssetsVE = new BuildReportPackedAssetsVE(report);
				PackedAssets_Content = packedAssetsVE.GetVE();
				Content.Add(PackedAssets_Content);
				break;
		}
	}
	public void RegisterEvents()
	{
		BuildSteps_ToolbarBtn.RegisterCallback<ClickEvent>(OnBuildStepsClick);
		PackedAssets_ToolbarBtn.RegisterCallback<ClickEvent>(OnPackedAssetsClick);
	}

	private void OnPackedAssetsClick(ClickEvent evt)
	{
		selectedContetType = ContentType.PackedAssets;
		UpdateContentVE();
	}

	private void OnBuildStepsClick(ClickEvent evt)
	{
		selectedContetType = ContentType.BuildSteps;
		UpdateContentVE();
	}
	enum ContentType
	{
		BuildSteps,
		PackedAssets
	}
}
