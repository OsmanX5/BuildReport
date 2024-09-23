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

	VisualElement Platform_LabelIcon; // update in future to use Label icon type
	VisualElement SummaryResult_LabelIcon; 
	VisualElement TotalSize_IconInfo;
	VisualElement TotalTime_IconInfo;
	VisualElement TotalWarnings_IconInfo;
	VisualElement TotalErrors_IconInfo;
	BuildSummary summaryData;
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
		return result.Children().ToList()[0];
	}
	public void QueryVisualElements(VisualElement baseVE)
	{
		SummaryResult_LabelIcon = baseVE.Q<VisualElement>(nameof(SummaryResult_LabelIcon));
		Platform_LabelIcon = baseVE.Q<VisualElement>(nameof(Platform_LabelIcon));
		TotalSize_IconInfo = baseVE.Q<VisualElement>(nameof(TotalSize_IconInfo));
		TotalTime_IconInfo = baseVE.Q<VisualElement>(nameof(TotalTime_IconInfo));
		TotalWarnings_IconInfo = baseVE.Q<VisualElement>(nameof(TotalWarnings_IconInfo));
		TotalErrors_IconInfo = baseVE.Q<VisualElement>(nameof(TotalErrors_IconInfo));
	}

	private void SetSummeryResult(BuildResult result)
	{
		SummaryResult_LabelIcon.Q<Label>("Label").text = result.ToString();
		VisualElement iconVE = SummaryResult_LabelIcon.Q<VisualElement>("Icon");
		
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
		iconVE.style.backgroundImage = icon;

	}
	private void SetPlatform(BuildTarget platform)
	{
		VisualElement iconVE = Platform_LabelIcon.Q<VisualElement>("Icon");
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
		Platform_LabelIcon.Q<Label>("Label").text = resultStr;
		iconVE.style.backgroundImage = icon;
	}
	private void SetTotalize(ulong totalSize)
	{
		TotalSize_IconInfo.Q<Label>("Value").text = totalSize.FormatSize();
		TotalSize_IconInfo.Q<VisualElement>("Icon").style.backgroundImage = IconsLibrary.Instance.Core.GetIcon("Size");
	}
	private void SetTotalTime(TimeSpan totalTime)
	{
		TotalTime_IconInfo.Q<Label>("Value").text = totalTime.FormatTime();
		TotalTime_IconInfo.Q<VisualElement>("Icon").style.backgroundImage = IconsLibrary.Instance.Core.GetIcon("Time");
	}
	private void SetTotalErrors(int totalErrors)
	{
		TotalErrors_IconInfo.Q<Label>("Value").text = totalErrors.ToString();
		TotalErrors_IconInfo.Q<VisualElement>("Icon").style.backgroundImage = IconsLibrary.Instance.Core.GetIcon("Error");
	}

	private void SetTotalWarnings(int totalWarnings)
	{
		TotalWarnings_IconInfo.Q<Label>("Value").text = totalWarnings.ToString();
		TotalWarnings_IconInfo.Q<VisualElement>("Icon").style.backgroundImage = IconsLibrary.Instance.Core.GetIcon("Warning");
	}





}
