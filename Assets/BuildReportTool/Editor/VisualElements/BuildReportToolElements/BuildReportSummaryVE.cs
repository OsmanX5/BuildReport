using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportSummaryVE 
{
	const string templatePath = "Assets/BuildReportTool/Editor/VisualElements/BuildReportToolElements/BuildReportSummaryVE.uxml";
	BuildSummary summaryData;

	
	VisualElement CoreData_VE;

	IconedLabel Platform_IconedLabel; 
	IconedLabel SummaryResult_IconedLabel; 
	IconedLabel TotalSize_IconedLabel;
	IconedLabel TotalTime_IconedLabel;
	IconedLabel TotalWarnings_IconedLabel;
	IconedLabel TotalErrors_IconedLabel;

	public BuildReportSummaryVE(BuildSummary summaryData)
	{
		this.summaryData = summaryData;
    }

	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree =AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		SetSummeryResult(summaryData.result);
		SetPlatform(summaryData.platform);
		SetTotalize(summaryData.totalSize);
		SetTotalTime(summaryData.totalTime);
		SetTotalWarnings(summaryData.totalWarnings);
		SetTotalErrors(summaryData.totalErrors);
		AddCoreDataElements();
		return result.Children().ToList()[0];
	}


	public void QueryVisualElements(VisualElement baseVE)
	{
		CoreData_VE = baseVE.Q<VisualElement>("CoreData_VE");
	}
	private void AddCoreDataElements()
	{
		CoreData_VE.Add(Platform_IconedLabel);
		CoreData_VE.Add(SummaryResult_IconedLabel);
		CoreData_VE.Add(TotalSize_IconedLabel);
		CoreData_VE.Add(TotalTime_IconedLabel);
		CoreData_VE.Add(TotalWarnings_IconedLabel);
		CoreData_VE.Add(TotalErrors_IconedLabel);

	}

	private void SetSummeryResult(BuildResult result)
	{
		Texture2D icon = null;
		switch(result)
		{
			case BuildResult.Succeeded:
				icon = IconsLibrary.Instance.Core.GetIcon("Success");
				break;
			case BuildResult.Failed:
				icon = IconsLibrary.Instance.Core.GetIcon("Error");
				break;
			case BuildResult.Cancelled:
				icon = IconsLibrary.Instance.Core.GetIcon("Cancelled");
				break;
			case BuildResult.Unknown:
				icon = IconsLibrary.Instance.Core.GetIcon("Unknown");
				break;
		}
		SummaryResult_IconedLabel = new IconedLabel(icon, result.ToString());
	}
	private void SetPlatform(BuildTarget platform)
	{
		Texture2D icon = null;
		string resultStr = platform.ToString();
		switch (platform)
		{
			case BuildTarget.StandaloneWindows:
				icon = IconsLibrary.Instance.Platforms.GetIcon("Windows");
				resultStr = "Windows";
				break;
			case BuildTarget.StandaloneWindows64:
				icon = IconsLibrary.Instance.Platforms.GetIcon("Windows");

				break;
			case BuildTarget.Android:
				icon = IconsLibrary.Instance.Platforms.GetIcon("Android");
				break;
			case BuildTarget.iOS:
				icon = IconsLibrary.Instance.Platforms.GetIcon("iOS");
				break;
			case BuildTarget.WebGL:
				icon = IconsLibrary.Instance.Platforms.GetIcon("WebGL");
				break;
			default:
				icon = IconsLibrary.Instance.Platforms.GetIcon("Unknown");
				break;
		}
		Platform_IconedLabel = new IconedLabel(icon, resultStr);
	}
	private void SetTotalize(ulong totalSize)
	{
		Texture2D sizeIcon = IconsLibrary.Instance.Core.GetIcon("Size");
		TotalSize_IconedLabel = new IconedLabel(sizeIcon, totalSize.FormatSize());
	}
	private void SetTotalTime(TimeSpan totalTime)
	{
		Texture2D timeIcon = IconsLibrary.Instance.Core.GetIcon("Time");
		string TimeAsString = totalTime.FormatTime();
		TotalTime_IconedLabel = new IconedLabel(timeIcon, TimeAsString);
	}
	private void SetTotalErrors(int totalErrors)
	{
		Texture2D errorsIcon = IconsLibrary.Instance.Core.GetIcon("Error");
		TotalErrors_IconedLabel = new IconedLabel(errorsIcon, totalErrors.ToString());
	}

	private void SetTotalWarnings(int totalWarnings)
	{
		Texture2D warningIcon = IconsLibrary.Instance.Core.GetIcon("Warning");
		TotalWarnings_IconedLabel = new IconedLabel(warningIcon, totalWarnings.ToString());
	}
}
