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
	const string templatePath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/BuildReportPackedAssetsVE.uxml";
	const string packedAssetInfoItemVEpath = "Assets/BuildReportTool/Editor/CustomVisualElements/BuildReportToolElements/PackedAssetInfoItemVE.uxml";
	AssetsInfoLogic assetsInfoLogic;

	VisualElement SideOptionsVE;
	VisualElement SelectedAssetCardVE;
	ListView packedAssets_ListView;
	RadioButtonGroup SortBy_RadioBtnGroup;
	GroupBox TypesToShowChecks_GroupBox;
	Button SelecAll_Btn;
	Button SelectNone_Btn;

	Dictionary<Toggle, Type> TopTypeFilterToggels;
	Toggle othersCheck_Toggle;
	Slider MinAssetSize_Slider;
	Label MinAssetSize_Label;

	HashSet<Type> selectedFilters;
	SortByType sortBy = SortByType.Size;
	float MinSizeInMB => MinAssetSize_Slider.value;
	public BuildReportPackedAssetsVE(BuildReport buildReport)
	{
		assetsInfoLogic =new (buildReport);
	}

	public VisualElement GetVE()
	{
		VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
		VisualElement result = new VisualElement();
		visualTree.CloneTree(result);
		QueryVisualElements(result);
		CreateOptionesPanel();
		RegisterEvents();
		UpdateToShowItem();
		return result.Children().ToList()[0];
	}



	public void QueryVisualElements(VisualElement baseVE)
	{
		packedAssets_ListView = baseVE.Q<ListView>(nameof(packedAssets_ListView));
		
		SideOptionsVE = baseVE.Q<VisualElement>(nameof(SideOptionsVE));
		SortBy_RadioBtnGroup = SideOptionsVE.Q<RadioButtonGroup>(nameof(SortBy_RadioBtnGroup));
		TypesToShowChecks_GroupBox = SideOptionsVE.Q<GroupBox>(nameof(TypesToShowChecks_GroupBox));
		SelecAll_Btn = SideOptionsVE.Q<Button>(nameof(SelecAll_Btn));
		SelectNone_Btn = SideOptionsVE.Q<Button>(nameof(SelectNone_Btn));
		MinAssetSize_Slider = SideOptionsVE.Q<Slider>(nameof(MinAssetSize_Slider));
		MinAssetSize_Label = SideOptionsVE.Q<Label>(nameof(MinAssetSize_Label));

		SelectedAssetCardVE = baseVE.Q<VisualElement>(nameof(SelectedAssetCardVE));
		SelectedAssetCardVE.style.display = DisplayStyle.None;
	}
	private void CreateOptionesPanel()
	{
		CreateSortByItems();
		CreateShowOnlyChecks();
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

		var pieChart = new PieChartWithData(Types,values.ToArray(), colors);
		VisualElement pieChartVE = new VisualElement();
		pieChartVE.Add(pieChart);
		SideOptionsVE.Add(pieChartVE);		
	}

	private void CreateSortByItems()
	{
		SortBy_RadioBtnGroup.choices = sortByDict.Keys.Select(x => sortByDict[x].ToString()).ToList();
		SortBy_RadioBtnGroup.value = 0;
	}
	private void CreateShowOnlyChecks()
	{
		TopTypeFilterToggels = new Dictionary<Toggle, Type>();
		Dictionary<Type,TypeInfoData> topTypesToShow = assetsInfoLogic.TopTypesInfoData;
		foreach (var typeToShowData in topTypesToShow)
		{
			Type type = typeToShowData.Key;
			TypeInfoData typeInfoData = typeToShowData.Value;

			Toggle typeCheck_Toggle = new Toggle();
			CreateTypeFilterCheckBoxVE(typeCheck_Toggle, type, typeInfoData);
			TopTypeFilterToggels.Add(typeCheck_Toggle, type);
		}
		
		othersCheck_Toggle = new Toggle();
		CreateTypeFilterCheckBoxVE(othersCheck_Toggle, null, assetsInfoLogic.OtherTypesInfosData);
		

		VisualElement CreateTypeFilterCheckBoxVE(Toggle typeCheck_Toggle, Type? type, TypeInfoData typeInfoData)
		{
			string typeName = type == null ? "Others" : type.Name;

			VisualElement TypeFilterCheckBoxVE = new();
			TypeFilterCheckBoxVE.style.flexDirection = FlexDirection.Row;

			typeCheck_Toggle.text = $"{typeName}({typeInfoData.numberOfAssets})";
			typeCheck_Toggle.value = true;
			typeCheck_Toggle.style.flexGrow = 1;
			TypeFilterCheckBoxVE.Add(typeCheck_Toggle);

			Label typeSize = new($"{typeInfoData.TypeTotalSize.FormatSize()}");

			TypeFilterCheckBoxVE.Add(typeSize);
			TypesToShowChecks_GroupBox.Add(TypeFilterCheckBoxVE);

			return TypeFilterCheckBoxVE;

		}
	}

	public void RegisterEvents()
	{
		SortBy_RadioBtnGroup.RegisterValueChangedCallback(OnSortByRadioBtnGroupChanged);
		
		foreach(Toggle checkToggle in TopTypeFilterToggels.Keys)
			checkToggle.RegisterCallback<ChangeEvent<bool>>(OnFilterCheckedChange);
		othersCheck_Toggle.RegisterCallback<ChangeEvent<bool>>(OnFilterCheckedChange);

		SelecAll_Btn.clicked += OnSelectAllBtnClicked;
		SelectNone_Btn.clicked += OnSelectNoneBtnClicked;

		MinAssetSize_Slider.lowValue = assetsInfoLogic.MinAssetSizeInMB;
		MinAssetSize_Slider.highValue = assetsInfoLogic.MaxAssetSizeInMB;
		MinAssetSize_Slider.RegisterValueChangedCallback(OnMinAssetSizeSliderValueChanged);
	}

	private void OnMinAssetSizeSliderValueChanged(ChangeEvent<float> evt)
	{
		MinAssetSize_Label.text = $"{evt.newValue} MB";
		UpdateToShowItem();
	}

	private void OnSelectAllBtnClicked()
	{
		foreach(var toggle in TopTypeFilterToggels)
		{
			toggle.Key.value = true;
		}
		othersCheck_Toggle.value = true;
	}
	private void OnSelectNoneBtnClicked()
	{
		foreach (var toggle in TopTypeFilterToggels)
		{
			toggle.Key.value = false;
		}
		othersCheck_Toggle.value = false;
	}



	private void OnFilterCheckedChange(ChangeEvent<bool> evt)
	{
		UpdateToShowItem();
	}
	void UpdateSelectedFilters()
	{
		selectedFilters = new HashSet<Type>();
		foreach (var toggle in TopTypeFilterToggels)
		{
			if (toggle.Key.value == true)
				selectedFilters.Add(toggle.Value);
		}
		//Debug
	}

	private void OnSortByRadioBtnGroupChanged(ChangeEvent<int> evt)
	{
		sortBy = sortByDict[evt.newValue];
		UpdateToShowItem();
	}
	
	void SetPackedAssetsInfo(PackedAssetInfo[] assetsInfos)
	{
		packedAssets_ListView.itemsSource = assetsInfos;
		
		packedAssets_ListView.makeItem = packedAssetListViewItemCreator;
		packedAssets_ListView.bindItem = packedAssetListViewItemBinder;
		packedAssets_ListView.selectedIndicesChanged += OnItemClicked;
		packedAssets_ListView.itemsChosen += OnAssetFromListViewChosen;
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
		Label AssetName_Label = SelectedAssetCardVE.Q<Label>(nameof(AssetName_Label));
		Label AssetPath_Label = SelectedAssetCardVE.Q<Label>(nameof(AssetPath_Label));
		Label AssetSize_Label = SelectedAssetCardVE.Q<Label>(nameof(AssetSize_Label));
		ListView AssetUsingScenes_ListView = SelectedAssetCardVE.Q<ListView>(nameof(AssetUsingScenes_ListView));
		VisualElement AssetIcon_VE = SelectedAssetCardVE.Q<VisualElement>(nameof(AssetIcon_VE));

		PackedAssetInfo info = (PackedAssetInfo)packedAssets_ListView.itemsSource[index];

		AssetName_Label.text = Path.GetFileNameWithoutExtension(info.sourceAssetPath);
		AssetPath_Label.text = info.sourceAssetPath;
		AssetSize_Label.text = info.packedSize.FormatSize();
		Texture2D cashedIcon = new Texture2D(22, 22);
		Texture cahsed = AssetDatabase.GetCachedIcon(info.sourceAssetPath);
		if (cahsed != null)
			Graphics.ConvertTexture(cahsed, cashedIcon);
		AssetIcon_VE.style.backgroundImage = cashedIcon;
		string[] scenes = assetsInfoLogic.GetScenesForAsset(info.sourceAssetPath);
		if(scenes.Length == 0)
		{
			AssetUsingScenes_ListView.style.display = DisplayStyle.None;
		}
		else
		{
			AssetUsingScenes_ListView.style.display = DisplayStyle.Flex;

			AssetUsingScenes_ListView.itemsSource = scenes;
			AssetUsingScenes_ListView.makeItem = () => new SceneUsageListItemVE();
			AssetUsingScenes_ListView.bindItem = (element, i) => (element as SceneUsageListItemVE).SetData(scenes[i]);
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


	private void OnAssetFromListViewChosen(IEnumerable<object> enumerable)
	{
		foreach (PackedAssetInfo info in enumerable)
		{
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info.sourceAssetPath));
		}
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
		Texture2D cashedIcon = new Texture2D(22,22);
		Texture cahsed = AssetDatabase.GetCachedIcon(info.sourceAssetPath);
		if(cahsed != null)
			Graphics.ConvertTexture(cahsed, cashedIcon);
		CashedIconVE.style.backgroundImage = cashedIcon;

		Label TagLabel = element.Q<Label>("Tag");
        AssetInfoTag tag;
		if(info.sourceAssetPath.StartsWith("Assets"))
            tag = new AssetInfoTag { Tag = "Assets", Color = new Color(235 / 255f, 84 / 255f, 97 / 255f) }; 
        else if(info.sourceAssetPath.StartsWith("Packages"))
            tag = new AssetInfoTag { Tag = "Packages", Color = new Color(101/255f, 213 / 255f, 243 / 255f) };
        else
            tag = new AssetInfoTag { Tag = "Other", Color = Color.gray };;
        TagLabel.text = tag.Tag;
		TagLabel.style.backgroundColor = tag.Color;

        Label SizeVE = element.Q<Label>("Size");
        SizeVE.text = info.packedSize.FormatSize();

		VisualElement SceneUsageTagVE = element.Q<VisualElement>(nameof(SceneUsageTagVE));
		int sceneUsage = assetsInfoLogic.GetScenesForAsset(info.sourceAssetPath).Length;
		if(sceneUsage == 0)
			SceneUsageTagVE.style.display = DisplayStyle.None;
		else
			SceneUsageTagVE.style.display = DisplayStyle.Flex;
		Label ScenesCount_Label = SceneUsageTagVE.Q<Label>(nameof(ScenesCount_Label));
		ScenesCount_Label.text = sceneUsage.ToString();

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
	static Dictionary<int, SortByType> sortByDict  = new Dictionary<int, SortByType>
	{
		{0, SortByType.Size},
		{1, SortByType.Type},
		{2, SortByType.Name},
	};


	public void UpdateToShowItem()
	{
		UpdateSelectedFilters();
		PackedAssetInfo[] res = assetsInfoLogic.GetToShowItem(selectedFilters, sortBy, othersCheck_Toggle.value, MinSizeInMB);

		SetPackedAssetsInfo(res);
	}
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