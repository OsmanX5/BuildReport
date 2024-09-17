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
	public BuildReportPackedAssetsVE(PackedAssets[] packedAssets)
	{
		assetsInfoLogic =new (packedAssets);
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
		values.Add(assetsInfoLogic.OtherTypesInfosData.TypeTotalSize);
		var colors = new List<Color>();
		foreach (var x in typesAndData)
		{
			float HueValue = UnityEngine.Mathf.Lerp(0, 120/255f, 1- x.Value.Precentage);
			colors.Add(Color.HSVToRGB(HueValue, 1, 1));
		}
		colors.Add(Color.white);

		for(int i= 0; i < values.Count; i++)
		{
			Debug.Log($"item {i} precentage = {values[i]} ,Color = {colors[i]}");
		}
		var pieChart = new PieChart(values.ToArray(), colors.ToArray());
		SideOptionsVE.Add(pieChart);
		foreach(var x in typesAndData)
		{
			VisualElement temp = new VisualElement();
			temp.Add(new Label($"{x.Key} [{x.Value.Precentage}]"));
			SideOptionsVE.Add(temp);
		}
		
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

		packedAssets_ListView.itemsChosen += OnAssetFromListViewChosen;
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
		Label SizeVE = element.Q<Label>("Size");
		SizeVE.text = info.packedSize.FormatSize();

		Label NameVE = element.Q<Label>("Name");
		NameVE.text = Path.GetFileName(info.sourceAssetPath);
		VisualElement IconVE = element.Q<VisualElement>("Icon");
		Texture2D icon = IconsLibrary.Instance.Core.GetIcon("Unknown");
		if (TypesIcons.ContainsKey(info.type))
			icon = IconsLibrary.Instance.Types.GetIcon(TypesIcons[info.type]);
		else
			NameVE.text = $"[{info.type.Name}] NameVE.text";

		VisualElement CashedIconVE = element.Q<VisualElement>("CashedIcon");
		Texture2D cashedIcon = new Texture2D(22,22);
		Texture cahsed = AssetDatabase.GetCachedIcon(info.sourceAssetPath);
		if(cahsed != null)
			Graphics.ConvertTexture(cahsed, cashedIcon);
		CashedIconVE.style.backgroundImage = cashedIcon;
		IconVE.style.backgroundImage = icon;
	}


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
