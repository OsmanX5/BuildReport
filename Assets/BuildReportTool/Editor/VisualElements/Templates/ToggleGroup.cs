using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleGroup : VisualElement
{
	string choices;
	public string Choices
	{
		get { return choices; }
		set
		{
			choices = value;
			ChoicesList = choices.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
		}
	}
	
	string title;
	public string Title
	{
		get { return title; }
		set
		{
			title = value;
			TitleLabel.text = title;
		}
	}

	VisualElement toggelsListVE;
	Label TitleLabel;
	Button selectAllBtn;
	Button selectNoneBtn;

	List<string> choicesList;
	public List<string> ChoicesList
	{
		get
		{
			return choicesList;
		}
		set
		{
			choicesList = value;
			UpdateTogglesList();
		}
	}
	List<Toggle> Toggels;
	public event Action OnToggleGroupSelectionChanged;
	public ToggleGroup()
	{
		TitleLabel = new Label();
		Add(TitleLabel);

		toggelsListVE = new VisualElement();
		choicesList = new List<string>();
		Add(toggelsListVE);


		VisualElement buttonsContainer = new VisualElement();
		buttonsContainer.style.flexDirection = FlexDirection.Row;
		
		selectAllBtn = new Button(OnSelectAllBtnClicked);
		selectAllBtn.text = "Select All";
		buttonsContainer.Add(selectAllBtn);

		selectNoneBtn = new Button(OnSelectNoneBtnClicked);
		selectNoneBtn.text = "Select None";
		buttonsContainer.Add(selectNoneBtn);

		Add(buttonsContainer);
		ApplyingStyles();
	}

	void UpdateTogglesList()
	{
		toggelsListVE.Clear();
		Toggels = new List<Toggle>();
		foreach (var choice in choicesList)
		{
			Toggle toggle = new Toggle();
			toggle.text = choice;
			toggle.RegisterValueChangedCallback(OnAnyToggleValueChanged);
			toggelsListVE.Add(toggle);
			Toggels.Add(toggle);
		}
	}
	private void OnAnyToggleValueChanged(ChangeEvent<bool> evt)
	{
		OnToggleGroupSelectionChanged?.Invoke();
	}


	void OnSelectAllBtnClicked()
	{
		SelectAll();
	}
	void OnSelectNoneBtnClicked()
	{
		foreach (var toggle in Toggels)
		{
			toggle.value = false;
		}
	}
	public bool[] GetToggelsStates()
	{
		return Toggels.Select(x => x.value).ToArray();
	}
	public void ApplyingStyles()
	{
		TitleLabel.style.color = Color.white;
		TitleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

		selectAllBtn.focusable = false;
		selectNoneBtn.focusable = false;
	}

	internal void SelectAll()
	{
		foreach (var toggle in Toggels)
		{
			toggle.value = true;
		}
	}

	public new class UxmlFactory : UxmlFactory<ToggleGroup, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		UxmlStringAttributeDescription choices = new UxmlStringAttributeDescription { name = "choices" };
		UxmlStringAttributeDescription title = new UxmlStringAttributeDescription { name = "title" };
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get { yield break; }
		}
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			((ToggleGroup)ve).Choices = choices.GetValueFromBag(bag, cc);
			((ToggleGroup)ve).Title = title.GetValueFromBag(bag, cc);
		}
	}
}
