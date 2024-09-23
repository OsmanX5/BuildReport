using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Reporting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildReportTool : EditorWindow
{
	[SerializeField]
	private VisualTreeAsset m_VisualTreeAsset = default;

	/// <summary>
	/// Visual Elements
	/// </summary>
	ObjectField buildReport_ObjectField;
	Button NewBuild_Btn;
	Button SaveReport_Btn;
	Button ContactMe_Btn;
	VisualElement Body_VE;
	VisualElement BuildReportContent_VE;
	VisualElement root;
	/// <summary>
	/// Logic Data
	/// </summary>
	BuildReport buildReport => buildReport_ObjectField.value as BuildReport;

	[MenuItem("BuildTools/BuildReportTool")]
	public static void ShowBuildReportToolWindow()
	{
		BuildReportTool wnd = GetWindow<BuildReportTool>();
		
	}

	private void OnDisable()
	{
		SaveLastOpenedReport();

	}

	public void CreateGUI()
	{
		m_VisualTreeAsset.CloneTree(rootVisualElement);
		root = rootVisualElement;
		QueryElements();
		TryLoadLastOpenedReport();
		RegisterEvents();
	}

	void QueryElements()
	{
		Body_VE = root.Q<VisualElement>("Body_VE");
		buildReport_ObjectField = Body_VE.Q<ObjectField>("BuildReport_ObjectField");
		BuildReportContent_VE = Body_VE.Q<VisualElement>("BuildReportContent_VE");
		NewBuild_Btn = Body_VE.Q<Button>(nameof(NewBuild_Btn));
		SaveReport_Btn = Body_VE.Q<Button>(nameof(SaveReport_Btn));
		ContactMe_Btn =root.Q<Button>("ContactMe_Btn");
	}
	void RegisterEvents()
	{
		buildReport_ObjectField.RegisterCallback<ChangeEvent<Object>>(OnBuildReportChanged);
		NewBuild_Btn.clicked += OnNewBuildClicked;
		SaveReport_Btn.clicked += OnSaveReportClicked;
		ContactMe_Btn.clicked += OnContactMeClicked;
	}

	private void OnContactMeClicked()
	{
		string URL = "https://www.linkedin.com/in/osman-elfaki/";
		Application.OpenURL(URL);
	}

	private void OnSaveReportClicked()
	{
		if(ValidateOpenLastBuild() == false)
		{
			EditorUtility.DisplayDialog("Error", "There is no build report to save try to make a build first", "Ok");
			return;
		}
		string fileSavePath = EditorUtility.SaveFilePanelInProject("Save Build Report", "Assets", "BuildReport", "buildreport");
		Debug.Log("fileSavePath: " + fileSavePath);
		var temp = ImportLastBuildReport(fileSavePath);
		buildReport_ObjectField.value = temp;
	}

	private void OnNewBuildClicked()
	{
		BuildPlayerOptions final = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
		final.options |= BuildOptions.DetailedBuildReport;
		string Scenes = "";
		int c = 0;
		foreach (var scene in final.scenes)
		{
			Scenes += $"[{c++}]{scene} \n";
		}
		if(EditorUtility.DisplayDialog("Build Report Tool", "Building These scesne \n" + Scenes, "Ok"))
		{
			BuildOptions options = final.options;
			options |= BuildOptions.CleanBuildCache;
			var lastBuild = BuildPipeline.BuildPlayer(final.scenes,final.locationPathName,final.target,options);
			lastBuild.name = "LastBuild";
			buildReport_ObjectField.value = lastBuild;

			Debug.Log("end building");
		}

	}
	public static bool ValidateOpenLastBuild()
	{
		return File.Exists("Library/LastBuild.buildreport");
	}

	public static BuildReport ImportLastBuildReport(string assetPath)
	{
		File.Copy("Library/LastBuild.buildreport", assetPath, true);
		AssetDatabase.ImportAsset(assetPath);
		BuildReport saved = AssetDatabase.LoadAssetAtPath<BuildReport>(assetPath);
		saved.name = Path.GetFileNameWithoutExtension(assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		return saved;
	}
	private void OnBuildReportChanged(ChangeEvent<Object> evt)
	{
		Debug.Log("Build Report Changed");

		ReBuildTheBody();
	}

	void ReBuildTheBody()
	{

		BuildReportContent_VE.Clear();
		if(buildReport == null)
		{
			this.Repaint();
			return;
		}
		VisualElement ReportSummary = new BuildReportSummaryVE(buildReport.summary).GetVE();
		ReportSummary.name = "ReportSummary_this";
		BuildReportContent_VE.Add(ReportSummary);
		VisualElement ReportDeepContent = new BuildReportDeepContentVE(buildReport).GetVE();
		BuildReportContent_VE.Add(ReportDeepContent);
		this.Repaint();
	}

	const string EDITOR_PREFS_LAST_OPENED_REPORT = "BuildReportTool_LastOpenedReport";
	void SaveLastOpenedReport()
	{
		if(buildReport == null)
			return;
		string lastBuildReporPath = AssetDatabase.GetAssetPath(buildReport);
		EditorPrefs.SetString(EDITOR_PREFS_LAST_OPENED_REPORT, lastBuildReporPath);
	}
	void TryLoadLastOpenedReport()
	{
		string path = EditorPrefs.GetString(EDITOR_PREFS_LAST_OPENED_REPORT);
		if (string.IsNullOrEmpty(path))
			return;
		BuildReport report = AssetDatabase.LoadAssetAtPath<BuildReport>(path);
		if(report != null)
		{
			buildReport_ObjectField.value = report;
			ReBuildTheBody();
		}
	}

}
