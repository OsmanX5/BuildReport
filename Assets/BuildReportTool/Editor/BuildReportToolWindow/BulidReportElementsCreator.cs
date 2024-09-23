using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public static class BulidReportElementsCreator
{
	/// <summary>
	/// build summary data includes
	///  1 - platform
	///  2 - result
	///  3 - total size
	///  4 - total errors
	///  5 - total warnings
	///  6 - path
	/// </summary>
	/// <param name="summary"></param>
	/// <returns></returns>
	public static VisualElement BuildSummary(BuildSummary summary, VisualElement SummeryVE)
	{
		var root = new VisualElement();
		root.Add(new Label("Build Summary"));
		root.Add(new Label($"Build Result: {summary.result}"));
		root.Add(new Label($"Total Size: {summary.totalSize}"));
		root.Add(new Label($"Total Time: {summary.totalTime}"));
		root.Add(new Label($"Total Errors: {summary.totalErrors}"));
		root.Add(new Label($"Total Warnings: {summary.totalWarnings}"));
		Debug.Log($"Build Result: {summary.result}");
		return root;
	}
	public static VisualElement BuildSteps(BuildStep[] steps)
	{
		var root = new VisualElement();
		root.Add(new Label("Build Steps"));

		foreach (var step in steps)
		{
			var stepElement = new VisualElement();
			stepElement.Add(new Label($"Step Name: {step.name} <=> Duration: {step.duration} "));
			root.Add(stepElement);
		}

		return root;
	}

	
    public static VisualElement BuildFiles(BuildFile[] assets)
	{
		var root = new VisualElement();
		root.Add(new Label("Build Assets"));

		foreach (var asset in assets)
		{
			var assetElement = new VisualElement();
			assetElement.Add(new Label($"Asset Name: {asset.id} <=> Size: {asset.size} "));
			root.Add(assetElement);
		}

		return root;
	}

	internal static VisualElement BuildAssets(PackedAssets[] packedAssets)
	{
		var root = new VisualElement();
		root.Add(new Label("Packed Assets"));

		foreach (var packedAsset in packedAssets)
		{
			var packedAssetElement = new VisualElement();
			packedAssetElement.Add(new Label($"Asset Name: {packedAsset.name} <=> Size: {packedAsset.contents} "));
			root.Add(packedAssetElement);
		}

		return root;
	}
}

public class LabelIcon64
{
	VisualElement LableIcon64VE;
	public LabelIcon64(VisualElement root,string label, string iconPath)
	{
		LableIcon64VE = root;
		Label label_UI = LableIcon64VE.Q<Label>("Label");
		VisualElement icon_UI = LableIcon64VE.Q<VisualElement>("Icon");
		label_UI.text = label;
		icon_UI.style.backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
	}
	public VisualElement GetVisualElement()
	{
		return LableIcon64VE;
	}
}
