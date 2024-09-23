using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectedAssetCardVE : VisualElement
{
	/// <summary>
	/// Data Variables
	/// </summary>
	public static VisualTreeAsset visualTreeAsset;
	public string AssetName;
	public string AssetSize;
	public string AssetType;
	public string AssetPath;
	public Texture2D AssetPreview;

	/// <summary>
	/// Visual Elements
	/// </summary>
	Label AssetName_Label;
	Label AssetPath_Label;
	Label AssetSize_Label;
	Label AssetType_Label;
	VisualElement CustomInfoVE;
	ListView AssetUsingScenes_ListView ;
	VisualElement AssetIcon_VE;

	public SelectedAssetCardVE()
	{
		if (visualTreeAsset == null)
			visualTreeAsset = VisualElementUtilities.LoadUXML("SelectedAssetCardVE");
		if (visualTreeAsset == null)
			Debug.LogError("SelectedAssetCardVE VisualTreeAsset is null");
		visualTreeAsset.CloneTree(this);
		AssetName_Label = this.Q<Label>(nameof(AssetName_Label));
		AssetPath_Label = this.Q<Label>(nameof(AssetPath_Label));
		AssetSize_Label = this.Q<Label>(nameof(AssetSize_Label));
		AssetType_Label = this.Q<Label>(nameof(AssetType_Label));
		CustomInfoVE = this.Q<VisualElement>(nameof(CustomInfoVE));
		AssetUsingScenes_ListView = this.Q<ListView>(nameof(AssetUsingScenes_ListView));
		AssetIcon_VE = this.Q<VisualElement>(nameof(AssetIcon_VE));
	}
	public new class UxmlFactory : UxmlFactory<SelectedAssetCardVE, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		UxmlStringAttributeDescription assetName = new UxmlStringAttributeDescription { name = "asset-name" };
		UxmlStringAttributeDescription assetSize = new UxmlStringAttributeDescription { name = "asset-size" };
		UxmlStringAttributeDescription assetType = new UxmlStringAttributeDescription { name = "asset-type" };
		UxmlStringAttributeDescription assetPath = new UxmlStringAttributeDescription { name = "asset-path" };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			SelectedAssetCardVE element = ve as SelectedAssetCardVE;
			element.AssetName = assetName.GetValueFromBag(bag, cc);
			element.AssetSize = assetSize.GetValueFromBag(bag, cc);
			element.AssetType = assetType.GetValueFromBag(bag, cc);
			element.AssetPath = assetPath.GetValueFromBag(bag, cc);
		}
	}
}
