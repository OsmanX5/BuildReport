using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;

public class BuildStepsInfoLogic 
{

	
	BuildStep[] buildSteps;
	public BuildStepsInfoLogic(BuildStep[] steps)
	{
		buildSteps= steps;
	}

	public List<string> GetBuildStepNodes()
	{
		List<string> nodes = new List<string>();
		foreach (var step in buildSteps)
		{
			nodes.Add($"{step.name} :{step.depth} , mc :{step.messages.Length}");
		}
		return nodes;
	}
	private static List<LogType> ErrorLogTypes = new List<LogType> { LogType.Error, LogType.Assert, LogType.Exception };

	public static LogType WorseLogType(LogType log1, LogType log2)
	{
		if (ErrorLogTypes.Contains(log1) || ErrorLogTypes.Contains(log2))
			return LogType.Error;
		if (log1 == LogType.Warning || log2 == LogType.Warning)
			return LogType.Warning;
		return LogType.Log;
	}

}

public class BuildStepNode
	{
		
	}