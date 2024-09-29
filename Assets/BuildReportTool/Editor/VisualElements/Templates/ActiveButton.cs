using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActiveButton : Button
{

    Color pressedColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f);
	public Color PressedColor
	{
		get => pressedColor;
		set
		{
			pressedColor = value;
		}
	}

	Color unPressedColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f);
	public Color UnPressedColor
	{
		get => unPressedColor;
		set
		{
			unPressedColor = value;
		}
	}

	bool isPressed;
	public bool IsPressed
	{
		get => isPressed;
		set
		{
			isPressed = value;
			if (isPressed)
			{
				this.style.backgroundColor = pressedColor;
			}
			else
			{
				this.style.backgroundColor = unPressedColor;
			}
		}
	}
	
	Color currentColor => IsPressed ? pressedColor : unPressedColor;

	public ActiveButton()
	{
		this.clicked += OnClicked;
		this.RegisterCallback<MouseOverEvent>(onHover);
		this.RegisterCallback<MouseOutEvent>(onHoverOut);
		IsPressed = false;

	}

	private void onHoverOut(MouseOutEvent evt)
	{
		this.style.backgroundColor = currentColor;
	}

	private void onHover(MouseOverEvent evt)
	{
		this.style.backgroundColor = currentColor.Highlight();
	}

	public new class UxmlFactory : UxmlFactory<ActiveButton, UxmlTraits> { }
    public new class UxmlTraits : Button.UxmlTraits {
		UxmlColorAttributeDescription m_PressedColor = new UxmlColorAttributeDescription { 
			name = "pressed-color" ,
			defaultValue = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f) };
		UxmlColorAttributeDescription m_UnPressedColor = new UxmlColorAttributeDescription { 
			name = "unpressed-color", 
			defaultValue = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f) };
		UxmlBoolAttributeDescription m_IsPressed = new UxmlBoolAttributeDescription { name = "is-pressed" };
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			ActiveButton button = (ActiveButton)ve;
			button.PressedColor = m_PressedColor.GetValueFromBag(bag, cc);
			button.UnPressedColor = m_UnPressedColor.GetValueFromBag(bag, cc);
			button.IsPressed = m_IsPressed.GetValueFromBag(bag, cc);
		}
	}
	public void OnClicked()
	{
		IsPressed = !IsPressed;
	}
}
