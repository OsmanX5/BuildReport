using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class IconedLabel : VisualElement
{
	string _text;
	public string Text
	{
		get => _text;
		set
		{
			_text = value;
			label.text = _text;
		}
	}

	
	string _iconPath;
	public string IconPath
	{
		get => _iconPath;
		set
		{
			_iconPath = value;
			if (!string.IsNullOrEmpty(_iconPath))
			{
				Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(_iconPath);
			}
		}
	}

	public enum IconSizes
	{
		Small = 16,
		Medium = 32,
		Large = 48,
		XLarge = 64,
		XXLarge = 128,
		XXXLarge = 256,
		XXXXLarge = 512
	}
	public IconSizes IconSize
	{
		get => iconSize;
		set
		{
			iconSize = value;
			icon.style.width = (int)iconSize;
			icon.style.height = (int)iconSize;
			if(iconSize == IconSizes.Small )
			{
				this.style.flexDirection = FlexDirection.Row;
			}
			else
			{
				this.style.flexDirection = FlexDirection.Column;
			}
		}
	}

	Texture2D _icon;
	public Texture2D Icon
	{
		get => _icon;
		set
		{
			_icon = value;
			icon.style.backgroundImage = _icon;
		}
	}

	IconSizes iconSize = IconSizes.Medium;


	Label label;
	VisualElement icon;
	public IconedLabel()
	{
		label = new Label();
		icon = new Label();
		Add(icon);
		Add(label);
		ApplyStyling();
	}
	public IconedLabel(string iconPath,string text= "", IconSizes iconSize = IconSizes.Medium) : this()
	{
		Text = text;
		IconPath = iconPath;
	}
	public IconedLabel(Texture2D icon, string text = "", IconSizes iconSize = IconSizes.Medium) : this()
	{
		Text = text;
		Icon = icon;
		IconSize = iconSize;
	}
	private void ApplyStyling()
	{
		this.style.flexWrap = Wrap.Wrap;
		this.style.alignItems = Align.Center;
		int margin = 2;
		this.style.marginBottom = margin;
		this.style.marginTop = margin;
		this.style.marginLeft = margin;
		this.style.marginRight = margin;

		icon.style.flexShrink = 0;

		label.style.whiteSpace = WhiteSpace.Normal;
		label.style.unityTextAlign = TextAnchor.MiddleLeft;
	}

	public new class UxmlFactory : UxmlFactory<IconedLabel, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		UxmlStringAttributeDescription _textAtr = new() { name = "text", defaultValue = "Text" };
		UxmlStringAttributeDescription _iconAtr = new() { name = "Icon Path", defaultValue = "" };
		UxmlEnumAttributeDescription<IconSizes> _iconSizeAtr = new() { name = "Icon Size", defaultValue = IconSizes.Medium };
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			IconedLabel il = (IconedLabel)ve;
			il.Text = _textAtr.GetValueFromBag(bag, cc);
			il.IconPath = _iconAtr.GetValueFromBag(bag, cc);
			il.IconSize = _iconSizeAtr.GetValueFromBag(bag, cc);
		}
	}
}
