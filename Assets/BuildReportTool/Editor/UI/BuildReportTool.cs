using System.ComponentModel;
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
	}
	void RegisterEvents()
	{
		buildReport_ObjectField.RegisterCallback<ChangeEvent<Object>>(OnBuildReportChanged);
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
