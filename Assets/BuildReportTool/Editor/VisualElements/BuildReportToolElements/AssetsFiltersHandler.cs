using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetsFiltersHandler : VisualElement
{
	VisualElement FiltersBaseVE;

	// Sort By
	RadioButtonGroup SortBy_RadioBtnGroup;
	SortByType sortBy = SortByType.Size;
	static Dictionary<int, SortByType> sortByDict = new Dictionary<int, SortByType>
	{
		{0, SortByType.Size},
		{1, SortByType.Type},
		{2, SortByType.Name},
	};

	// Types To Show
	ToggleGroup Types_ToggleGroup;

	// Size
	MinMaxSlider AssetSize_MinMaxSlider;
	float MinSizeInMB => AssetSize_MinMaxSlider.minValue;
	float MaxSizeInMB => AssetSize_MinMaxSlider.maxValue;


	// Folders
	ActiveButton Assets_ActiveButton;
	ActiveButton Packges_ActiveButton;
	ActiveButton Others_ActiveButton;
	TagsSelector tagsSelector;

	AssetsInfoLogic assetsInfoLogic;
	AssetsFiltersData assetsFiltersData;
	public AssetsFiltersData FiltersData => assetsFiltersData;
	public event Action<AssetsFiltersData> OnFiltersDataUpdated;
	public AssetsFiltersHandler(AssetsInfoLogic assetsInfoLogic)
	{
		assetsFiltersData = new AssetsFiltersData();
		this.assetsInfoLogic = assetsInfoLogic;
		
	}
	public void SetBaseVE(VisualElement baseVE)
	{
		FiltersBaseVE = baseVE;
		QueryElements();
		SetupSortBy();
		SetupTypesToShowFilters();
		SetupAssetsSize();
	}


	#region SortBy

	void SetupSortBy()
	{
		SortBy_RadioBtnGroup = FiltersBaseVE.Q<RadioButtonGroup>(nameof(SortBy_RadioBtnGroup));
		CreateSortByItems();
		SortBy_RadioBtnGroup.RegisterValueChangedCallback(OnSortByRadioBtnGroupChanged);
	}
	private void CreateSortByItems()
	{
		SortBy_RadioBtnGroup.choices = sortByDict.Keys.Select(x => sortByDict[x].ToString()).ToList();
		SortBy_RadioBtnGroup.value = 0;
	}
	private void OnSortByRadioBtnGroupChanged(ChangeEvent<int> evt)
	{
		sortBy = sortByDict[evt.newValue];
		assetsFiltersData.SortByType = sortBy;
		FiltersDataUpdated();
	}


	#endregion

	#region TypesToShow
	
	Type[] CheckListTypes;
	void SetupTypesToShowFilters()
	{
		Types_ToggleGroup = FiltersBaseVE.Q<ToggleGroup>(nameof(Types_ToggleGroup));
		var TopTypes =  assetsInfoLogic.TopTypesInfoData;
		List<string> TypesAsString = TopTypes.Select(x => $"{x.Key.Name}({x.Value.numberOfAssets}) = {x.Value.TypeTotalSize.FormatSize()}").ToList();
		TypesAsString.Add($"Others({assetsInfoLogic.OtherTypesInfosData.numberOfAssets}) = {assetsInfoLogic.OtherTypesInfosData.TypeTotalSize.FormatSize()}");
		CheckListTypes = TopTypes.Keys.ToArray();
		Types_ToggleGroup.ChoicesList = TypesAsString;
		Types_ToggleGroup.OnToggleGroupSelectionChanged += OnSelectedTypesChanged;
		assetsFiltersData.IncludedTypes = new HashSet<Type>(CheckListTypes);
		Types_ToggleGroup.SelectAll();
	}

	private void OnSelectedTypesChanged()
	{
		var selected = Types_ToggleGroup.GetToggelsStates();
		int last = selected.Length - 1;
		HashSet<Type> selectedTypes = new HashSet<Type>();
		for (int i=0 ; i<selected.Length-1; i++) // last is the other types
		{
			if (selected[i])
				selectedTypes.Add(CheckListTypes[i]);
		}
		assetsFiltersData.IncludedTypes = selectedTypes;
		assetsFiltersData.OtheTypes = selected[last];
		FiltersDataUpdated();
	}

	#endregion

	#region Size
	string MinMaxSizeAsString => $"{MinSizeInMB.ToString("0.00")}MB - {MaxSizeInMB.ToString("0.00")}MB";
	private void SetupAssetsSize()
	{
		AssetSize_MinMaxSlider = FiltersBaseVE.Q<MinMaxSlider>(nameof(AssetSize_MinMaxSlider));
		
		
		AssetSize_MinMaxSlider.highLimit = assetsInfoLogic.MaxAssetSizeInMB;
		AssetSize_MinMaxSlider.lowLimit = assetsInfoLogic.MinAssetSizeInMB;
		AssetSize_MinMaxSlider.minValue = assetsInfoLogic.MinAssetSizeInMB;
		AssetSize_MinMaxSlider.maxValue = assetsInfoLogic.MaxAssetSizeInMB;
		AssetSize_MinMaxSlider.label = MinMaxSizeAsString;
		AssetSize_MinMaxSlider.RegisterCallback<ChangeEvent<Vector2>>(MinMiaxValueChanged);
		
	}

	private void MinMiaxValueChanged(ChangeEvent<Vector2> newValue)
	{
		AssetSize_MinMaxSlider.label = MinMaxSizeAsString;
		assetsFiltersData.MinAssetSize = MinSizeInMB;
		assetsFiltersData.MaxAssetSize = MaxSizeInMB;
		FiltersDataUpdated();
	}




	#endregion

	void QueryElements()
	{
		
		Assets_ActiveButton = FiltersBaseVE.Q<ActiveButton>(nameof(Assets_ActiveButton));
		Assets_ActiveButton.IsPressed = true;
		Packges_ActiveButton = FiltersBaseVE.Q<ActiveButton>(nameof(Packges_ActiveButton));
		Packges_ActiveButton.IsPressed = false;

		Others_ActiveButton = FiltersBaseVE.Q<ActiveButton>(nameof(Others_ActiveButton));
		Others_ActiveButton.IsPressed = false;

		tagsSelector = new TagsSelector();
		tagsSelector.AddButton(Assets_ActiveButton);
		tagsSelector.AddButton(Packges_ActiveButton);
		tagsSelector.AddButton(Others_ActiveButton);
	}

	public void FiltersDataUpdated()
	{
		OnFiltersDataUpdated?.Invoke(assetsFiltersData);
	}
}

public class AssetsFiltersData
{
	public SortByType SortByType = SortByType.Size;
	public HashSet<Type> IncludedTypes = new HashSet<Type>();
	public bool OtheTypes;
	public float MinAssetSize;
	public float MaxAssetSize;
	public bool IncludeAssetsFolder;
	public bool IncludePachagesFolder;
	public bool IncludeOtherFolders;
}