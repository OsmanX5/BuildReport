using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TiteledLabel : VisualElement
{
	string _title;
	public string Title
	{
		get => _title;
		set
		{
			_title = value;
			TitelLabel.text = Title;
		}
	}

	string _label;
	public string Label
	{
		get => _label;
		set
		{
			_label = value;
			LabelLabel.text = _label;
		}
	}
	int _baseFontSize ;
	public int BaseFontSize
	{
		get => _baseFontSize;
		set
		{
			_baseFontSize = value;
			TitelLabel.style.fontSize = BaseFontSize +2;
			LabelLabel.style.fontSize = BaseFontSize;
		}
	}
	Label TitelLabel;
	Label LabelLabel;
	public TiteledLabel()
	{
		TitelLabel = new Label();
		LabelLabel = new Label();
		Add(TitelLabel);
		Add(LabelLabel);
		ApplyStyling();
	}
	public TiteledLabel(string title, string label, int  fontSize = 14) : this()
	{
		Title = title;
		Label = label;
		BaseFontSize = fontSize;
	}
	public new class UxmlFactory : UxmlFactory<TiteledLabel, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits {
		UxmlStringAttributeDescription _titleAtr = new () { name = "title" ,defaultValue = "Title"};
		UxmlStringAttributeDescription _labelAtr = new () { name = "label" ,defaultValue ="Label"};
		UxmlIntAttributeDescription _fontSizeAtr = new () { name = "fontSize", defaultValue = 14 };
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TiteledLabel titeledLabel = (TiteledLabel)ve;
			titeledLabel.Title = _titleAtr.GetValueFromBag(bag, cc);
			titeledLabel.Label = _labelAtr.GetValueFromBag(bag, cc);
			titeledLabel.BaseFontSize = _fontSizeAtr.GetValueFromBag(bag, cc);
		}
	}

	/// <summary>
	/// to replace in future with a stylesheet
	/// </summary>
	void ApplyStyling()
	{
		this.style.flexDirection = FlexDirection.Row;
		this.style.flexWrap = Wrap.Wrap;
		this.style.alignItems = Align.Center;
		TitelLabel.style.flexWrap = Wrap.Wrap;
		TitelLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

		LabelLabel.style.flexWrap = Wrap.Wrap;
		LabelLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
		LabelLabel.style.whiteSpace = WhiteSpace.Normal;

	}
}
