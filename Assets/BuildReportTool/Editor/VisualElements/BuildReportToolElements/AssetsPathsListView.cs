using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetsPathsListView : VisualElement
{
	ListView Assets_ListView;

	public AssetsPathsListView(string[] AssetPaths)
	{
		string[] ordered = AssetPaths.OrderByDescending(x => new FileInfo(x).Length).ToArray();
		SetPackedAssetsInfo(ordered);
		Add(Assets_ListView);
	}

	void SetPackedAssetsInfo(string[] assetsPaths)
	{
		Assets_ListView = new ListView();
		Assets_ListView.fixedItemHeight = 30;
		Assets_ListView.itemsSource = assetsPaths;
		Assets_ListView.makeItem = packedAssetListViewItemCreator;
		Assets_ListView.bindItem = packedAssetListViewItemBinder;
		Assets_ListView.itemsChosen += OnAssetFromListViewChosen;
		Assets_ListView.Rebuild();
	}
	private VisualElement packedAssetListViewItemCreator()
	{
		VisualElement ve = new VisualElement();
		ve.style.flexDirection = FlexDirection.Row;
		VisualElement CashedIcon = new VisualElement();
		CashedIcon.name = "CashedIcon";
		int margin = 2;
		CashedIcon.style.width = Assets_ListView.fixedItemHeight - 2 * margin;
		CashedIcon.style.height = Assets_ListView.fixedItemHeight - 2 * margin;
		CashedIcon.style.marginBottom = 2;CashedIcon.style.marginTop = 2;CashedIcon.style.marginLeft = 2; CashedIcon.style.marginRight = 2;
		CashedIcon.style.flexShrink = 0;
		
		Label Name = new Label();
		Name.name = "Name";
        Name.style.flexGrow = 1;
        Label SizeInPoroject = new Label("");
		SizeInPoroject.name = "Size";
		ve.Add(CashedIcon);
		ve.Add(Name);
		ve.Add(SizeInPoroject);
		return ve;
	}
	private void packedAssetListViewItemBinder(VisualElement element, int index)
	{
		string path = (string)Assets_ListView.itemsSource[index];


		Label NameVE = element.Q<Label>("Name");
		string name = Path.GetFileNameWithoutExtension(path);
		NameVE.text = name;


		VisualElement CashedIconVE = element.Q<VisualElement>("CashedIcon");
		Texture2D cashedIcon = new Texture2D(22, 22);
		Texture cahsed = AssetDatabase.GetCachedIcon(path);
		if (cahsed != null)
			Graphics.ConvertTexture(cahsed, cashedIcon);
		CashedIconVE.style.backgroundImage = cashedIcon;

        Label SizeInPoroject = element.Q<Label>("Size");
		string size = (new FileInfo(path)).Length.FormatSize();
		SizeInPoroject.text= size;

    }
	private void OnAssetFromListViewChosen(IEnumerable<object> enumerable)
	{
		foreach (string path in enumerable)
		{
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
		}
	}
}
