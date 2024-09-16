using Codice.Client.BaseCommands;
using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;

[ExecuteInEditMode]
public class AssetsInfoLogic 
{
	const int MAX_ASSETS_TO_SHOW = 100;
	const int NUMBER_OF_TOP_TYPES = 8;
	PackedAssetInfo[] toShowAssetsInfo;	
	public Dictionary<Type, TypeInfoData> TopTypesInfoData { get; private set; }
	public TypeInfoData OtherTypesInfosData { get; private set; }
	List<Type> OtherTypes;
	ulong minAssetSize = 0;
	ulong MaxAssetSize = 0;
	
	public float MinAssetSizeInMB
	{
		get => minAssetSize.GetSizeInMB();
	}
	public float MaxAssetSizeInMB
	{
		get => MaxAssetSize.GetSizeInMB();
	}
	public AssetsInfoLogic(IEnumerable<PackedAssets> packedAssets)
	{
		List<PackedAssetInfo> temp = new List<PackedAssetInfo>();
		foreach (PackedAssets packedAsset in packedAssets)
			temp.AddRange(packedAsset.contents);
		Init(temp);
	}
	Dictionary<Type, PackedAssetInfo[]> allPackedAssetsInfoGrouped;
	PackedAssetInfo[] allPackedAssetsInfoSorted;
	void Init(IEnumerable<PackedAssetInfo> packedAssetsInfo)
	{
		// Cash All Assets
		allPackedAssetsInfoSorted = packedAssetsInfo.OrderByDescending(x => x.packedSize).ToArray();
		allPackedAssetsInfoGrouped = allPackedAssetsInfoSorted.GroupBy(x => x.type).ToDictionary(x => x.Key, x => x.ToArray());


		// Get Each Type Info
		Dictionary<Type,TypeInfoData> TypesInfo = allPackedAssetsInfoGrouped.
			Select(TypeAndAssets => new KeyValuePair<Type, TypeInfoData>(TypeAndAssets.Key, new TypeInfoData(TypeAndAssets.Value))).
			OrderByDescending(TypeInfoData => TypeInfoData.Value.totalSize).
			ToDictionary(x => x.Key, x => x.Value);

		TopTypesInfoData = TypesInfo.Take(NUMBER_OF_TOP_TYPES).ToDictionary(dict => dict.Key, dict => dict.Value);

		var otherTypesInfos = TypesInfo.Skip(NUMBER_OF_TOP_TYPES).ToDictionary(dict => dict.Key, dict => dict.Value);
		OtherTypes = otherTypesInfos.Keys.ToList();
		
		ulong otherTypesTotalSize = (ulong)otherTypesInfos.Values.Sum(x => (decimal)x.totalSize);
		OtherTypesInfosData = new(otherTypesInfos.Count(), otherTypesTotalSize);

		minAssetSize = allPackedAssetsInfoSorted.Min(x => x.packedSize);
		MaxAssetSize = allPackedAssetsInfoSorted.Max(x => x.packedSize);
	}
	public PackedAssetInfo[] GetToShowItem(HashSet<Type> selectedTypesFilters , SortByType sortBy, bool showOtherTypesFilters,float MinSizeInMB = 0)
	{
		toShowAssetsInfo = new PackedAssetInfo[MAX_ASSETS_TO_SHOW];
		int addedAssets = 0;
		for(int i=0; i< allPackedAssetsInfoSorted.Length; i++)
		{
			if (addedAssets >= MAX_ASSETS_TO_SHOW)
				break;
			if (!selectedTypesFilters.Contains(allPackedAssetsInfoSorted[i].type))
				continue;
			if (allPackedAssetsInfoSorted[i].packedSize.GetSizeInMB() < MinSizeInMB)
				continue;
			toShowAssetsInfo[addedAssets] = allPackedAssetsInfoSorted[i];
			addedAssets++;
		}
		toShowAssetsInfo = toShowAssetsInfo.Take(addedAssets).ToArray();
		switch (sortBy)
		{
			case SortByType.Size:
				toShowAssetsInfo = toShowAssetsInfo.OrderBy(info => 0 - info.packedSize).ToArray();
				break;
			case SortByType.Name:
				toShowAssetsInfo = toShowAssetsInfo.OrderBy(info => info.sourceAssetPath).ToArray();
				break;
			case SortByType.Type:
				toShowAssetsInfo = toShowAssetsInfo.OrderBy(info => info.type.Name).ToArray();
				break;
			default:
				toShowAssetsInfo = toShowAssetsInfo.OrderBy(info => 0 - info.packedSize).ToArray();
				break;
		}
		return toShowAssetsInfo;
		
	}

}
public struct TypeInfoData
{
	public int numberOfAssets;
	public ulong totalSize;

	public TypeInfoData(int numberOfAssets, ulong totalSize)
	{
		this.numberOfAssets = numberOfAssets;
		this.totalSize = totalSize;
	}
	public TypeInfoData(IEnumerable<PackedAssetInfo> assets)
	{
		numberOfAssets = assets.Count();
		totalSize = 0;
		foreach (PackedAssetInfo asset in assets)
		{
			totalSize += asset.packedSize;
		}
	}

	public override string ToString()
	{
		return $"Number Of Assets: {numberOfAssets}, Total Size: {totalSize}";
	}
}
public enum SortByType
{
	Size,
	Name,
	Type,
}