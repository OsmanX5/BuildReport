using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class IconInfo : VisualElement
{
	public string TempleatePath = "Assets/BuildReportTool/Editor/CustomVisualElements/Templates/IconInfo.uxml";
	public string info;
	public Texture2D icon;

	public IconInfo(string info, Texture2D icon)
	{
		this.info = info;
		if (icon == null)
		{
			icon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;
		}
		this.icon = icon;

		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TempleatePath);
		if(visualTree == null)
		{
			Debug.LogError("Template not found");
			return;
		}
		VisualElement temp = visualTree.CloneTree();
		temp.Q<Label>("Value").text = info;
		temp.Q<VisualElement>("Icon").style.backgroundImage = icon;
		this.Add(temp);
	}
}
