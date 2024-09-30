using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportPackedAssetsVE : VisualElement
{
	//const string templatePath = "Assets/BuildReportTool/Editor/VisualElements/BuildReportToolWindows/BuildReportPackedAssetsVE.uxml";
	const string packedAssetInfoItemVEpath = "Assets/BuildReportTool/Editor/VisualElements/BuildReportToolElements/PackedAssetInfoItemVE.uxml";
	AssetsInfoLogic assetsInfoLogic;
	AssetsFiltersHandler assetsFiltersHandler;
	VisualElement SideOptionsVE;
	VisualElement SelectedAssetCardVE;
	PieChartWithData typePieChart;
	Foldout FiltersFoldout;
	ListView packedAssets_ListView;

	public BuildReportPackedAssetsVE(BuildReport buildReport)
	{
		assetsInfoLogic =new (buildReport);
		assetsFiltersHandler = new AssetsFiltersHandler(assetsInfoLogic);
		assetsFiltersHandler.OnFiltersDataUpdated += UpdateToShowItem;
	}

	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree = VisualElementUtilities.LoadUXML("BuildReportPackedAssetsVE");
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		assetsFiltersHandler.SetBaseVE(FiltersFoldout);
		SetPackedAssetsInfo(assetsInfoLogic.GetToShowItem());
		return result.Children().ToList()[0];
	}



	public void QueryVisualElements(VisualElement baseVE)
	{
		packedAssets_ListView = baseVE.Q<ListView>(nameof(packedAssets_ListView));
		SideOptionsVE = baseVE.Q<VisualElement>(nameof(SideOptionsVE));
		SelectedAssetCardVE = baseVE.Q<VisualElement>(nameof(SelectedAssetCardVE));
		SelectedAssetCardVE.style.display = DisplayStyle.None;
		FiltersFoldout = baseVE.Q<Foldout>(nameof(FiltersFoldout));
		typePieChart = baseVE.Q<PieChartWithData>(nameof(typePieChart));
		CreatePieChart();
	}

	private void CreatePieChart()
	{
		var typesAndData = assetsInfoLogic.TopTypesInfoData;

		var values = typesAndData.Select(x => (float)x.Value.Precentage).ToList();
		values.Add(assetsInfoLogic.OtherTypesInfosData.Precentage);
		int numOfColors = values.Count;
		Color[] colors = ColorsPalet1.Take(numOfColors).ToArray();
		if(numOfColors > ColorsPalet1.Count)
		{
            Debug.LogWarning("Not enough colors in the palet");
        }
		colors = colors.Select(colors => new Color(colors.r, colors.g, colors.b, 0.8f)).ToArray();
		var Types = typesAndData.Select(x => x.Key).ToList();
		Types.Add(null);
		typePieChart.SetData(Types,values.ToArray(), colors);
	}

	public void UpdateToShowItem(AssetsFiltersData filters)
	{
		PackedAssetInfo[] toShow = assetsInfoLogic.GetToShowItem(filters);
		SetPackedAssetsInfo(toShow);
	}
	void SetPackedAssetsInfo(PackedAssetInfo[] assetsInfos)
	{
		packedAssets_ListView.itemsSource = assetsInfos;
		packedAssets_ListView.makeItem = packedAssetListViewItemCreator;
		packedAssets_ListView.bindItem = packedAssetListViewItemBinder;
		packedAssets_ListView.selectedIndicesChanged += OnItemClicked;
		packedAssets_ListView.itemsChosen += OnAssetFromListViewChosen;
	}
	private VisualElement packedAssetListViewItemCreator()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(packedAssetInfoItemVEpath);
		if (visualTree == null)
		{
			Debug.Log("packedAssetInfoItemVEpath is null");
		}
		return visualTree.Instantiate().Children().First();
	}
	private void packedAssetListViewItemBinder(VisualElement element, int index)
	{
		PackedAssetInfo info = (PackedAssetInfo)packedAssets_ListView.itemsSource[index];


		Label NameVE = element.Q<Label>("Name");
		string name = Path.GetFileNameWithoutExtension(info.sourceAssetPath);
		NameVE.text = name;


		VisualElement CashedIconVE = element.Q<VisualElement>("CashedIcon");
		Texture2D cashedIcon = new Texture2D(22, 22);
		Texture cahsed = AssetDatabase.GetCachedIcon(info.sourceAssetPath);
		if (cahsed != null)
			Graphics.ConvertTexture(cahsed, cashedIcon);
		CashedIconVE.style.backgroundImage = cashedIcon;

		Label TagLabel = element.Q<Label>("Tag");
		AssetInfoTag tag;
		if (info.sourceAssetPath.StartsWith("Assets"))
			tag = new AssetInfoTag { Tag = "Assets", Color = new Color(235 / 255f, 84 / 255f, 97 / 255f) };
		else if (info.sourceAssetPath.StartsWith("Packages"))
			tag = new AssetInfoTag { Tag = "Packages", Color = new Color(101 / 255f, 213 / 255f, 243 / 255f) };
		else
			tag = new AssetInfoTag { Tag = "Other", Color = Color.gray }; ;
		TagLabel.text = tag.Tag;
		TagLabel.style.backgroundColor = tag.Color;

		Label SizeVE = element.Q<Label>("Size");
		SizeVE.text = info.packedSize.FormatSize();

		VisualElement SceneUsageTagVE = element.Q<VisualElement>(nameof(SceneUsageTagVE));
		int sceneUsage = assetsInfoLogic.GetScenesForAsset(info.sourceAssetPath).Length;
		if (sceneUsage == 0)
			SceneUsageTagVE.style.display = DisplayStyle.None;
		else
			SceneUsageTagVE.style.display = DisplayStyle.Flex;
		Label ScenesCount_Label = SceneUsageTagVE.Q<Label>(nameof(ScenesCount_Label));
		ScenesCount_Label.text = sceneUsage.ToString();

	}
	private void OnItemClicked(IEnumerable<int> enumerable)
	{
		SelectedAssetCardVE.style.display = DisplayStyle.Flex;
		foreach (int index in enumerable)
		{
			UpdateTheAssetCardWithThisInde(index);
		}
	}
	private void UpdateTheAssetCardWithThisInde(int index)
	{
		PackedAssetInfo info = (PackedAssetInfo)packedAssets_ListView.itemsSource[index];
		string[] scenes = assetsInfoLogic.GetScenesForAsset(info.sourceAssetPath);
		SelectedAssetCardVE newCard = new SelectedAssetCardVE();
		newCard.SetData(info, scenes);
		SelectedAssetCardVE.Clear();
		SelectedAssetCardVE.Add(newCard);
	}
	private void OnAssetFromListViewChosen(IEnumerable<object> enumerable)
	{
		foreach (PackedAssetInfo info in enumerable)
		{
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info.sourceAssetPath));
		}
	}

	struct AssetInfoTag
	{
		public string Tag;
		public Color Color;
	}
	List<Color> ColorsPalet1 = new List<Color>
	{
		new Color(235/255f,84/255f,97/255f,0/.7f),
		new Color(76/255f,175/255f,80/255f,0.7f),
		Color.cyan,
		Color.yellow,
		Color.magenta,
		Color.blue,
		Color.grey,
		Color.white,
		new Color(0.5f, 0.5f, 0.5f),
		new Color(0.5f, 0.5f, 0.5f),
		new Color(0.5f, 0.5f, 0.5f),
		new Color(0.5f, 0.5f, 0.5f),
	};

	static Dictionary<Type,string> TypesIcons = new Dictionary<Type, string>
	{
		{typeof(Texture2D), "Texture"},
		{typeof(Sprite), "Texture"},
		{typeof(MonoScript), "Script"},
		{typeof(Material), "Material"},
		{typeof(Shader), "Shader"},
		{typeof(AudioClip), "Audio"},
		{typeof(Font), "Font"},
		{typeof(TextAsset), "Text"},
	};
}
class SceneUsageListItemVE : VisualElement
{
	public SceneUsageListItemVE()
	{

	}
	public void SetData(string sceneSTR)
	{
		this.Clear();
		string[] Splited = sceneSTR.Split('(');
		string sceneNumber = Splited[0];
		string scenePath = Splited[1].Replace(")", "");
		string sceneName = scenePath.Split('/').Last();

		Label NumberLabel = new Label(sceneNumber);
		NumberLabel.style.backgroundColor = Color.white;
		NumberLabel.style.color = Color.black;
		int margin = 2;
		NumberLabel.style.marginBottom = margin;
		NumberLabel.style.marginTop = margin;
		NumberLabel.style.marginLeft = margin;
		NumberLabel.style.marginRight = margin;
		NumberLabel.style.paddingBottom = margin;
		NumberLabel.style.paddingTop = margin;
		NumberLabel.style.paddingLeft = margin;
		NumberLabel.style.paddingRight = margin;


		Label NameLabel = new Label(sceneName);
		NameLabel.style.flexGrow = 1;
		NameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
		Label PathLabel = new Label(scenePath);
		PathLabel.style.flexGrow = 1;
		PathLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
		this.style.flexDirection = FlexDirection.Row;


		VisualElement UnityIcon = new VisualElement();
		UnityIcon.style.width = 22;
		UnityIcon.style.height = 22;
		Texture2D icon = IconsLibrary.Instance.Core.GetIcon("UnityWhite");
		UnityIcon.style.backgroundImage = icon;
		Add(UnityIcon);
		Add(NameLabel);
		Add(NumberLabel);
		Add(PathLabel);
	}
}