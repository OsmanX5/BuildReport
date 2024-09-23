using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieChartVE : VisualElement
{
	float m_Radius = 100.0f;
	float m_Value = 40.0f;
	float[] m_Values ;
	Color[] m_colors;
	public float radius
	{
		get => m_Radius;
		set
		{
			m_Radius = value;
		}
	}

	public float diameter => m_Radius * 2.0f;

	public float value
	{
		get { return m_Value; }
		set { m_Value = value; MarkDirtyRepaint(); }
	}

	public PieChartVE(float[] precnets, Color[] colors)
	{
		this.m_colors = colors;
		m_Values = precnets;
		generateVisualContent += DrawCanvas;
		this.style.width = diameter;
		this.style.height = diameter;
	}

	void DrawCanvas(MeshGenerationContext ctx)
	{
		var painter = ctx.painter2D;
		painter.strokeColor = Color.white;
		painter.fillColor = Color.white;
		float angle = 0.0f;
		float anglePct = 0.0f;
		int k = 0;
		foreach (var pct in m_Values)
		{
			anglePct += 360.0f * (pct);
			painter.fillColor = m_colors[k++];
			painter.BeginPath();
			painter.MoveTo(new Vector2(m_Radius, m_Radius));
			painter.Arc(new Vector2(m_Radius, m_Radius), m_Radius, angle, anglePct);
			painter.Fill();
			painter.strokeColor = Color.black;
			painter.Stroke();

			angle = anglePct;
		}
	}
}
public class PieChartWithData : VisualElement
{
	PieChartVE m_PieChart;
    public PieChartWithData(List<Type> Types, float[] precnets, Color[] colors) 
    {
        m_PieChart = new PieChartVE(precnets, colors);
        Add(m_PieChart);
		for(int i = 0; i < precnets.Length; i++)
			Add(PieChartElementData(Types[i],precnets[i], colors[i]));
    }
	VisualElement PieChartElementData(Type type,float precent, Color color)
	{
		VisualElement ve = new VisualElement();
        ve.style.flexDirection = FlexDirection.Row;
		ve.style.marginBottom = 5;
		ve.style.marginLeft = 5;
		ve.style.marginRight = 5;
		ve.style.marginTop = 5;

        VisualElement colorBox = new VisualElement();
		colorBox.style.backgroundColor = color;
		colorBox.style.width = 20;
		colorBox.style.height = 20;
		colorBox.style.marginRight = 5;
		colorBox.style.marginLeft= 5;
		colorBox.style.borderRightWidth = 1;
		colorBox.style.borderRightColor = Color.black;
		colorBox.style.borderTopWidth = 1;
		colorBox.style.borderTopColor = Color.black;
		colorBox.style.borderBottomWidth = 1;
		colorBox.style.borderBottomColor = Color.black;
		colorBox.style.borderLeftWidth = 1;
		colorBox.style.borderLeftColor = Color.black;

		ve.Add(colorBox);

		Label TypeNameLabel = new Label();
		TypeNameLabel.text = type==null? "Others" : type.Name;
        TypeNameLabel.style.flexGrow = 1;
		ve.Add(TypeNameLabel);

        Label precentLabel = new Label($"{(precent*100).ToString("0.0")} %");
		ve.Add(precentLabel);
		return ve;
	}
}