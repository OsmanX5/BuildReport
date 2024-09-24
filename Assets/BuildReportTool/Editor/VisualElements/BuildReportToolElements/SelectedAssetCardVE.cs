using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;


public class SelectedAssetCardVE : VisualElement
{
	/// <summary>
	/// Visual Elements
	/// </summary>
	Label AssetName_Label;
	Label AssetPath_Label;
	Label AssetSize_Label;
	Label AssetType_Label;
	VisualElement CustomInfoVE;
	ListView AssetUsingScenes_ListView;
	VisualElement AssetIcon_VE;

	UnityEngine.Object assetObject;
	public UnityEngine.Object AssetObject
	{
		get => assetObject;
		private set
		{
			assetObject = value;
		}
	}
	PackedAssetInfo buildInfo;
	public PackedAssetInfo BuildInfo
	{
		get => buildInfo;
		private set
		{
			buildInfo = value;
		}
	}
	string[] scenePaths;
	public string[] ScenePaths
	{
		get => scenePaths;
		private set
		{
			scenePaths = value;
		}
	}

	string assetName;
	ulong assetSizeOnBuild;
	Type assetType;
	string assetPath;
	private VisualTreeAsset m_VisualTreeAsset = default;

	public SelectedAssetCardVE()
	{
		m_VisualTreeAsset = VisualElementUtilities.LoadUXML("SelectedAssetCardVE");
		if (m_VisualTreeAsset == null)
		{
			Debug.LogError("SelectedAssetCardVE VisualTreeAsset is null");
			return;
		}
		m_VisualTreeAsset.CloneTree(this);
		QueryElements();
	}
	public void SetData(UnityEngine.Object assetObject)
	{
		AssetObject = assetObject;
		UpdateObjectDataAndView();
	}
	public void SetData(PackedAssetInfo buildInfo, string[] scenePaths = null)
	{
		BuildInfo = buildInfo;
		AssetObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(buildInfo.sourceAssetPath);
		UpdateObjectDataAndView();
		UpdateBuildInfoDataAndView();
		if(scenePaths != null)
		{
			ScenePaths = scenePaths;
			UpdateSceneUsageInfoAndView();
		}
	}


	private void QueryElements()
	{
		AssetName_Label = this.Q<Label>(nameof(AssetName_Label));
		AssetPath_Label = this.Q<Label>(nameof(AssetPath_Label));
		AssetSize_Label = this.Q<Label>(nameof(AssetSize_Label));
		AssetType_Label = this.Q<Label>(nameof(AssetType_Label));
		CustomInfoVE = this.Q<VisualElement>(nameof(CustomInfoVE));
		AssetUsingScenes_ListView = this.Q<ListView>(nameof(AssetUsingScenes_ListView));
		AssetIcon_VE = this.Q<VisualElement>(nameof(AssetIcon_VE));

	}
	void UpdateObjectDataAndView()
	{
		
		assetPath = AssetDatabase.GetAssetPath(assetObject);
		AssetPath_Label.text = assetPath;


		assetName = Path.GetFileNameWithoutExtension(assetPath);
		AssetName_Label.text = assetName;
		
		
		try
		{
			assetType = assetObject.GetType();
			AssetType_Label.text = $"{assetType.Name}  [{assetType.ToString()}]";
		}
		catch (Exception e)
		{
			Debug.Log("UnKnown Type");
			AssetType_Label.text = "UnKnown Type";
			return;
		}



		Texture2D previewIcon = AssetPreview.GetAssetPreview(assetObject);
		AssetIcon_VE.style.backgroundImage = previewIcon;
		CustomInfoVE.style.display = DisplayStyle.None;
		CustomInfoVE.Clear();
		if (assetType.IsSubclassOf(typeof(Mesh)))
		{

			CustomInfoVE.style.display = DisplayStyle.Flex;
			Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
			int vertexCount = mesh.vertexCount;
			CustomInfoVE.Add(new Label($"Vertex Count: {vertexCount}"));

		}
		else if (assetType.IsSubclassOf(typeof(Texture)))
		{
			CustomInfoVE.style.display = DisplayStyle.Flex;
			Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
			if (texture == null)
			{
				return;
			}
			int textureSize = texture.width;
			int textureHeight = texture.height;
			Debug.Log(textureSize);
			CustomInfoVE.Add(new Label($"Texture Size : {textureSize} X {textureHeight}"));
		}		
	}
	void UpdateBuildInfoDataAndView()
	{
		assetSizeOnBuild = buildInfo.packedSize;
		AssetSize_Label.text = assetSizeOnBuild.FormatSize();

	}
	void UpdateSceneUsageInfoAndView()
	{
		if (scenePaths == null || scenePaths.Length == 0)
		{
			AssetUsingScenes_ListView.style.display = DisplayStyle.None;
		}
		else
		{
			AssetUsingScenes_ListView.style.display = DisplayStyle.Flex;

			AssetUsingScenes_ListView.itemsSource = scenePaths;
			AssetUsingScenes_ListView.makeItem = () => new SceneUsageListItemVE();
			AssetUsingScenes_ListView.bindItem = (element, i) => (element as SceneUsageListItemVE).SetData(scenePaths[i]);
			AssetUsingScenes_ListView.itemsChosen += (enumerable) =>
			{
				foreach (string selected in enumerable)
				{
					string path = selected.Split('(')[1].Replace(")", "");
					EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
				}
			};
			AssetUsingScenes_ListView.Rebuild();
		}
	}
}
