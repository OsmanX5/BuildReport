using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DetailedBuildReport : MonoBehaviour
{
	[MenuItem("Build/DetailedBuildReport example")]
	public static void MyBuild()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = new[] { "Assets/scene.unity" };
		buildPlayerOptions.locationPathName = "DetailedReportBuild/MyGame.exe";
		buildPlayerOptions.target = BuildTarget.StandaloneWindows64;

		buildPlayerOptions.options = BuildOptions.DetailedBuildReport;

		var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}