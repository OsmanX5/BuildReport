using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "IconsLibrary", menuName = "Build Report Tool/Icons Library", order = 1)]
public class IconsLibrary : ScriptableObject
{

	public IconLibrary Core;
	public IconLibrary Types;
	public IconLibrary Platforms;
	const string InstanceAssetsPath = "Assets/BuildReportTool/Editor/Assets/Icons/IconsLibrary.asset";
	static IconsLibrary instance;
	public static IconsLibrary Instance
	{
		get
		{
			if(instance == null)
			{
				instance = AssetDatabase.LoadAssetAtPath<IconsLibrary>(InstanceAssetsPath);
				if (instance == null)
				{
					Debug.LogError("IconsLibrary not found in the project, please create one");
				}
			}
			return instance;
		}
	}
}
[System.Serializable]
public class IconLibrary
{
	[SerializeField] List<Texture2D> Icons;
	string NotFoundIconPath = "Assets/BuildReportTool/Editor/Assets/Icons/Core/not-found.png";
	public Texture2D GetIcon(string name,int w= 64,int h = 64)
	{
		foreach (var item in Icons)
		{
			if (item.name == name)
			{
				
				return item;
			}
		}
		Debug.LogError($"Icon {name} not found in The Library ");
		Texture2D defaultIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(NotFoundIconPath);
		return defaultIcon;
	}

	public void LoadIcons(string folderName)
	{
		Icons = new List<Texture2D>();
		string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { $"Assets/BuildReportTool/Editor/Assets/Icons/{folderName}" });
		foreach (var guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			Icons.Add(icon);
		}
	}
}

[CustomEditor(typeof(IconsLibrary))]
public class IconsLibraryEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if(GUILayout.Button("Load Icons"))
		{
			IconsLibrary library = (IconsLibrary)target;
			library.Core.LoadIcons("Core");
			library.Types.LoadIcons("Types");
			library.Platforms.LoadIcons("Platforms");
			EditorUtility.SetDirty(library);
		}
	}
}