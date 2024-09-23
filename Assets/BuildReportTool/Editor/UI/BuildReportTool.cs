using System.ComponentModel;
using System.IO;
using UnityEditor;
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
	VisualElement Body_VE;
	VisualElement BuildReportContent_VE;
	VisualElement root;
	/// <summary>
	/// Logic Data
	/// </summary>
	BuildReport buildReport => buildReport_ObjectField.value as BuildReport;

	[MenuItem("CustomeTools/BuildReportTool")]
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
	}
	void RegisterEvents()
	{
		buildReport_ObjectField.RegisterCallback<ChangeEvent<Object>>(OnBuildReportChanged);
		NewBuild_Btn.clicked += OnNewBuildClicked;
		SaveReport_Btn.clicked += OnSaveReportClicked;
	}

	private void OnSaveReportClicked()
	{
		string path = EditorUtility.SaveFilePanel("Save Build Report", "Assets", "BuildReport", "buildreport");
		ImportLastBuildReport(path);
	}

	private void OnNewBuildClicked()
	{
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.options = BuildOptions.Development;
		options.options |= BuildOptions.DetailedBuildReport;
		Debug.Log("start building");
		Debug.Log(options.options);
		BuildPlayerOptions final = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);
		foreach (var scene in final.scenes)
		{
			Debug.Log(scene);
		}
		buildReport_ObjectField.value = BuildPipeline.BuildPlayer(final);

		Debug.Log("end building");
	}
	public static bool ValidateOpenLastBuild()
	{
		return File.Exists("Library/LastBuild.buildreport");
	}

	public static string ImportLastBuildReport(string assetPath)
	{
		File.Copy("Library/LastBuild.buildreport", assetPath, true);
		AssetDatabase.ImportAsset(assetPath);
		AssetDatabase.Refresh();
		return assetPath;
	}
	private void OnBuildReportChanged(ChangeEvent<Object> evt)
	{
		Debug.Log("Build Report Changed");
		ReBuildTheBody();
	}

	void ReBuildTheBody()
	{
		BuildReportContent_VE.Clear();

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
