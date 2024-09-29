using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class CustomExtensions
{
	public static string FormatTime(this System.TimeSpan t)
	{
		if (t.Hours == 0 && t.Minutes == 0)
			return t.Seconds + " s";
		if (t.Hours == 0)
			return t.Minutes + ":" + t.Seconds.ToString("D2") ;
		return t.Hours + ":" + t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2") ;
	}
	public static string FormatSize(this ulong size)
	{
		if (size < 1024)
			return size + " B";
		if (size < 1024 * 1024)
			return (size / 1024.00).ToString("F2") + " KB";
		if (size < 1024 * 1024 * 1024)
			return (size / (1024.0 * 1024.0)).ToString("F2") + " MB";
		return (size / (1024.0 * 1024.0 * 1024.0)).ToString("F2") + " GB";
	}
	public static float GetSizeInMB(this ulong size)
	{
		return (float)size / 1024 / 1024;
	}
	public static Color Highlight(this Color color, float val = 0.1f)
	{
		float r = Mathf.Clamp01(color.r + val);
		float g = Mathf.Clamp01(color.g + val);
		float b = Mathf.Clamp01(color.b + val);
		return new Color(r, g, b, color.a);
	}
}
