using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class CustomExtensions
{
	public static string FormatTime(this System.TimeSpan t)
	{
		return t.Hours + ":" + t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2") + "." + t.Milliseconds.ToString("D3");
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
}
