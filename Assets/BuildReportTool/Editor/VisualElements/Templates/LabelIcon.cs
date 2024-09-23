using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelIcon : BindableElement
{
	public string label;
	public Texture2D icon;

	private VisualElement Base;
	private Label Label_VE;
	private VisualElement Icon_VE;

	public LabelIcon(string label, Texture2D icon)
	{
		Base = CloneTemplate();
		QueryVisualElements(Base);
		SetData(label, icon);
		UpdateVisualElements();
		Add(Base);
	}

	private VisualElement CloneTemplate()
	{
		string TempleatePath = "Assets/BuildReportTool/Editor/CustomVisualElements/Templates/LabelIcon.uxml";
		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TempleatePath);
		if (visualTree == null)
		{
			Debug.LogError("Template not found");
			return null;
		}
		return visualTree.CloneTree();
	}

	private void QueryVisualElements(VisualElement temp)
	{
		Label_VE = temp.Q<Label>("Label");
		Icon_VE = temp.Q<VisualElement>("Icon");
	}

	public void SetData(string label, Texture2D icon)
	{
		this.label = label;
		if (icon == null)
		{
			icon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;
		}
		this.icon = icon;
	}
	void UpdateVisualElements()
	{
		Label_VE.text = label;
		Icon_VE.style.backgroundImage = icon;
	}
}
