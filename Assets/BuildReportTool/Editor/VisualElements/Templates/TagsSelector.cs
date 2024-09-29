using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TagsSelector 
{
	//To Do in future
	List<ActiveButton> buttons;
	List<string> selectedTags = new List<string>();
	public Action<List<string>> OnSelectedTagsChanged;
	public List<string> SelectedTags
	{
		get
		{
			selectedTags = new List<string>();
			foreach (var button in buttons)
			{
				if (button.IsPressed)
				{
					selectedTags.Add(button.text);
				}
				else
				{
					selectedTags.Remove(button.text);
				}
			}
			return selectedTags;
		}
	}
	public TagsSelector()
	{
		buttons = new List<ActiveButton>();
	}
	public TagsSelector(List<ActiveButton> buttons) : this()
	{
		foreach (var button in buttons)
		{
			this.buttons.Add(button);
		}
	}
	public void AddButton(ActiveButton button)
	{
		buttons.Add(button);
		if(button.IsPressed)
		{
			selectedTags.Add(button.text);
		}
		button.clicked += OnButtonClicked;
	}


	public void RemoveButton(ActiveButton button)
	{
		buttons.Remove(button);
		button.clicked -= OnButtonClicked;
	}
	private void OnButtonClicked()
	{
		OnSelectedTagsChanged?.Invoke(SelectedTags);
	}
}
