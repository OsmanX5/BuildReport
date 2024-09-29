using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AssetsInfoLogic 
{
	const int MAX_ASSETS_TO_SHOW = 100;
	const int NUMBER_OF_TOP_TYPES = 5;
	PackedAssetInfo[] toShowAssetsInfo;	
	public Dictionary<Type, TypeInfoData> TopTypesInfoData { get; private set; }
	Dictionary<string, string[]> AssetsScenesDict;
	public TypeInfoData OtherTypesInfosData { get; private set; }
	List<Type> OtherTypes;
	ulong minAssetSize = 0;
	ulong MaxAssetSize = 0;
	ulong TotalBuildSize;
	public float MinAssetSizeInMB
	{
		get => minAssetSize.GetSizeInMB();
	}
	public float MaxAssetSizeInMB
	{
		get => MaxAssetSize.GetSizeInMB();
	}
	public AssetsInfoLogic(BuildReport builreport)
	{
		var packedAssets = builreport.packedAssets;
		CreateSceneDict(builreport.scenesUsingAssets);
		List<PackedAssetInfo> temp = new List<PackedAssetInfo>();
		foreach (PackedAssets packedAsset in packedAssets)
			temp.AddRange(packedAsset.contents);
		Init(temp);
	}
	Dictionary<Type, PackedAssetInfo[]> allPackedAssetsInfoGrouped;
	PackedAssetInfo[] allPackedAssetsInfoSorted;
	void Init(IEnumerable<PackedAssetInfo> packedAssetsInfo)
	{
		if(packedAssetsInfo.Count() == 0)
		{
			TotalBuildSize = 0;
			TopTypesInfoData = new Dictionary<Type, TypeInfoData>();
			OtherTypesInfosData = new(0, 0, 0);
			OtherTypes = new List<Type>();
			Debug.LogWarning("No Assets Found");
			return;
		}
		// Cash All Assets
		allPackedAssetsInfoSorted = packedAssetsInfo.OrderByDescending(x => x.packedSize).ToArray();
		allPackedAssetsInfoGrouped = allPackedAssetsInfoSorted.GroupBy(x => x.type).ToDictionary(x => x.Key, x => x.ToArray());
		TotalBuildSize = (ulong)allPackedAssetsInfoSorted.Sum(x => (decimal)x.packedSize);
        // Get Each Type Info
        Dictionary<Type,TypeInfoData> TypesInfo = allPackedAssetsInfoGrouped.
			Select(TypeAndAssets => new KeyValuePair<Type, TypeInfoData>(TypeAndAssets.Key, new TypeInfoData(TypeAndAssets.Value, TotalBuildSize))).
			OrderByDescending(TypeInfoData => TypeInfoData.Value.TypeTotalSize).
			ToDictionary(x => x.Key, x => x.Value);

		TopTypesInfoData = TypesInfo.Take(NUMBER_OF_TOP_TYPES).ToDictionary(dict => dict.Key, dict => dict.Value);

		var otherTypesInfos = TypesInfo.Skip(NUMBER_OF_TOP_TYPES).ToDictionary(dict => dict.Key, dict => dict.Value);
		OtherTypes = otherTypesInfos.Keys.ToList();
		
		ulong otherTypesTotalSize = (ulong)otherTypesInfos.Values.Sum(x => (decimal)x.TypeTotalSize);
		OtherTypesInfosData = new(otherTypesInfos.Count(), otherTypesTotalSize, TotalBuildSize);
		
		minAssetSize = allPackedAssetsInfoSorted.Min(x => x.packedSize);
		MaxAssetSize = allPackedAssetsInfoSorted.Max(x => x.packedSize);
	}
	public PackedAssetInfo[] GetToShowItem(AssetsFiltersData filtersData = null)
	{
		toShowAssetsInfo = new PackedAssetInfo[MAX_ASSETS_TO_SHOW];
		int addedAssets = 0;
		if(filtersData == null)
		{
			return allPackedAssetsInfoSorted.Take(MAX_ASSETS_TO_SHOW).ToArray();
		}
		for(int i=0; i< allPackedAssetsInfoSorted.Length; i++)
		{
			PackedAssetInfo assetInfo = allPackedAssetsInfoSorted[i];
			if (addedAssets >= MAX_ASSETS_TO_SHOW)
				break;
			if (filtersData.IncludedTypes.Contains(assetInfo.type) == false)
				continue;
			if (assetInfo.packedSize.GetSizeInMB() < filtersData.MinAssetSize)
				continue;
			if (assetInfo.packedSize.GetSizeInMB() > filtersData.MaxAssetSize)
				continue;
            /*if (StartWithTags != null)
			{
				bool found = false;
				foreach (string tag in StartWithTags)
				{
					if (assetInfo.sourceAssetPath.StartsWith(tag))
					{
						found = true;
						break;
					}
				}
				if (!found)
					continue;
			}
            */
			toShowAssetsInfo[addedAssets] = assetInfo;
			addedAssets++;
		}
		toShowAssetsInfo = toShowAssetsInfo.Take(addedAssets).ToArray();
		switch (filtersData.SortByType)
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

	void CreateSceneDict(ScenesUsingAssets[] scenesUsingAssets)
	{
		
		AssetsScenesDict = new Dictionary<string, string[]>();
		try
		{
			foreach (ScenesUsingAssets scene in scenesUsingAssets)
			{
				foreach(var assetSceneInfo in scene.list)
				{
					AssetsScenesDict.Add(assetSceneInfo.assetPath, assetSceneInfo.scenePaths);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Error in CreateSceneDict: " + e.Message);
		}
	}

	public string[] GetScenesForAsset(string assetPath)
	{
		if (AssetsScenesDict.ContainsKey(assetPath))
			return AssetsScenesDict[assetPath];
		return new string[0];
	}
	public string[] GetScenesForAsset(PackedAssetInfo assetInfo)
	{
		return GetScenesForAsset(assetInfo.sourceAssetPath);
	}

}
public struct TypeInfoData
{
	public int numberOfAssets;
	public ulong TypeTotalSize;
	ulong BuildSize;
	public float Precentage => (float)TypeTotalSize / BuildSize;
	public TypeInfoData(int numberOfAssets, ulong totalSize,ulong buildSize)
	{
		this.numberOfAssets = numberOfAssets;
		this.TypeTotalSize = totalSize;
		this.BuildSize = buildSize;
	}
	public TypeInfoData(IEnumerable<PackedAssetInfo> assets,ulong buildSize)
	{
		numberOfAssets = assets.Count();
		TypeTotalSize = 0;
		foreach (PackedAssetInfo asset in assets)
		{
			TypeTotalSize += asset.packedSize;
		}
		BuildSize = buildSize;
	}

	public override string ToString()
	{
		return $"Number Of Assets: {numberOfAssets}, Total Size: {TypeTotalSize}";
	}
}
public enum SortByType
{
	Size,
	Name,
	Type,
}