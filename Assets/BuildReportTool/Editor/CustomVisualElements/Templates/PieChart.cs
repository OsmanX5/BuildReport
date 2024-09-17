using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieChart : VisualElement
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

	public PieChart(float[] precnets, Color[] colors)
	{
		this.m_colors = colors;
		m_Values = precnets;
		generateVisualContent += DrawCanvas;
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
			Debug.Log(pct);
			anglePct += 360.0f * (pct);
			painter.fillColor = m_colors[k++];
			painter.BeginPath();
			painter.MoveTo(new Vector2(m_Radius, m_Radius));
			painter.Arc(new Vector2(m_Radius, m_Radius), m_Radius, angle, anglePct);
			painter.Fill();

			angle = anglePct;
		}
	}
}