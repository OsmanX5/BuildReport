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
	#region Visual ELements Variables
	
	VisualTreeAsset m_VisualTreeAsset = default;
	UnityObjectPreview UnityObjectPreview;

	TiteledLabel AssetPath_TitledLabel;
	TiteledLabel AssetType_TitledLabel;
	Label AssetSize_Label;
	VisualElement CustomInfoVE;
	ListView AssetUsingScenes_ListView;


	#endregion

	#region Logic Variables
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
	#endregion


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
	private void QueryElements()
	{
		UnityObjectPreview = this.Q<UnityObjectPreview>(nameof(UnityObjectPreview));
		AssetSize_Label = this.Q<Label>(nameof(AssetSize_Label));
		
		AssetPath_TitledLabel = this.Q<TiteledLabel>(nameof(AssetPath_TitledLabel));
		AssetType_TitledLabel = this.Q<TiteledLabel>(nameof(AssetType_TitledLabel));

		AssetUsingScenes_ListView = this.Q<ListView>(nameof(AssetUsingScenes_ListView));
		CustomInfoVE = this.Q<VisualElement>(nameof(CustomInfoVE));
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
		if(AssetObject == null)
		{
			Debug.LogError("Asset Object is null");
			return;
		}
		UpdateObjectDataAndView();
		UpdateBuildInfoDataAndView();
		if(scenePaths != null)
		{
			ScenePaths = scenePaths;
			UpdateSceneUsageInfoAndView();
		}
		AddDependancyList();
	}

	private void AddDependancyList()
	{
		string[] dependancies = AssetDatabase.GetDependencies(assetPath);
		foreach (string dependancy in dependancies)
		{
			CustomInfoVE.Add(new Label(dependancy));
		}
	}

	void UpdateObjectDataAndView()
	{
		UnityObjectPreview.TargetObject  = assetObject;
		try
		{
			assetPath = AssetDatabase.GetAssetPath(assetObject);
			AssetPath_TitledLabel.Label = assetPath;
		}
		catch (Exception e)
		{
			AssetPath_TitledLabel.Label = "Didn't Find Asset Path check if it is valid Unity Object";
			return;
		}
		assetName = Path.GetFileNameWithoutExtension(assetPath);

		try
		{
			assetType = assetObject.GetType();
			AssetType_TitledLabel.Label= $"{assetType.Name}  [{assetType.ToString()}]";
		}
		catch (Exception e)
		{
			Debug.Log("UnKnown Type");
			AssetType_TitledLabel.Label = "UnKnown Type";
			return;
		}


		CreateCustomTypeData();

	}

	private void CreateCustomTypeData()
	{
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
			TiteledLabel textureSizeLabel = new TiteledLabel("Texture Size", $"{textureSize} X {textureHeight}",10);
			CustomInfoVE.Add(textureSizeLabel);
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
