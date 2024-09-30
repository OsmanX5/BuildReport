using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityObjectPreview;

public class UnityObjectPreview : VisualElement
{
	Object targetObject;
	public Object TargetObject
	{
		get => targetObject;
		set
		{
			targetObject = value;
			UpdateViewContainerAsync();
		}
	}

	string targetObjectPath;
	public string TargetObjectPath
	{
		get => targetObjectPath;
		set
		{
			targetObjectPath = value;
			if (!string.IsNullOrEmpty(targetObjectPath))
			{
				TargetObject = AssetDatabase.LoadAssetAtPath<Texture2D>(targetObjectPath);
			}
		}
	}
	public enum previewSizes
	{
		Small = 64,
		Medium = 128,
		Large = 256,
		XLarge = 512
	}
	previewSizes previewSize;
	public previewSizes PreviewSize
	{
		get => previewSize;
		set
		{
			previewSize = value;
			previewContainer.style.width = (int)previewSize;
			previewContainer.style.height = (int)previewSize;
		}
	}
	VisualElement previewContainer;
	Label previewLabel;
	public UnityObjectPreview()
	{
		previewContainer = new VisualElement();
		previewLabel = new Label();
		this.Add(previewContainer);
		this.Add(previewLabel);
		PreviewSize = previewSizes.Medium;
		ApplyStyling();
	}
	public UnityObjectPreview(string objectPath , previewSizes previewSize = previewSizes.Medium) : this()
	{
		TargetObjectPath = objectPath;
		PreviewSize = previewSize;
	}

	private void ApplyStyling()
	{
		this.style.flexDirection = FlexDirection.Column;
		this.style.alignItems = Align.Center;
		this.style.justifyContent = Justify.Center;
		previewContainer.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
	}
	public new class UxmlFactory : UxmlFactory<UnityObjectPreview, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		UxmlStringAttributeDescription objectPathAtr = new UxmlStringAttributeDescription { name = "object-path" };
		UxmlEnumAttributeDescription<previewSizes> previewSizeAtr = new UxmlEnumAttributeDescription<previewSizes> { name = "preview-size", defaultValue = previewSizes.Medium };
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get { yield break; }
		}
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			var preview = ve as UnityObjectPreview;
			preview.TargetObjectPath = objectPathAtr.GetValueFromBag(bag, cc);
			preview.PreviewSize = previewSizeAtr.GetValueFromBag(bag, cc);
		}
	}

	async void UpdateViewContainerAsync()
	{
		AssetPreview.GetAssetPreview(TargetObject);
		previewContainer.style.backgroundImage =  IconsLibrary.Instance.Core.GetIcon("Loading");
		while (AssetPreview.IsLoadingAssetPreview(TargetObject.GetInstanceID()))
		{
			await System.Threading.Tasks.Task.Delay(1000);
		}
		UpdatePreviewContainer();
	}
	void UpdatePreviewContainer()
	{
		previewContainer.Clear();
		if (TargetObject != null)
		{
			var preview = AssetPreview.GetAssetPreview(TargetObject);
			if (preview != null)
			{
				previewContainer.style.backgroundImage = preview;
			}
			previewLabel.text = TargetObject.name;
		}
	}
}
