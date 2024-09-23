using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;
using UnityEngine.UIElements;
using System;

public class BuildStepsInfoLogic 
{
	BuildStep[] buildSteps;
	LogicBuildStepNode logicBuildStepRoot;
	public BuildStepsInfoLogic(BuildStep[] steps)
	{
		buildSteps= steps;
		logicBuildStepRoot = CreateLogicBuildStepsNodes();
	}

	LogicBuildStepNode CreateLogicBuildStepsNodes()
	{
		Stack<LogicBuildStepNode> stack = new Stack<LogicBuildStepNode>();
		LogicBuildStepNode root = new LogicBuildStepNode(buildSteps[0],0);
		stack.Push(root);
		for (int i = 1; i < buildSteps.Length; i++)
		{
			BuildStep step = buildSteps[i];
			LogicBuildStepNode node = new LogicBuildStepNode(step,i);
			while (step.depth <= stack.Peek().depth)
			{
				stack.Pop();
			}
			if (step.depth > stack.Peek().depth)
			{
				stack.Peek().SubSteps.Add(node);
			}
			stack.Push(node);
		}
		return root;
	}

	List<TreeViewItemData<BuildNodeVEDataHolder>> CreateTreeViewItemsList(LogicBuildStepNode root)
	{
		List<TreeViewItemData<BuildNodeVEDataHolder>> treeViewItemDatas = new();
		SortedDictionary<int, TreeViewItemData<BuildNodeVEDataHolder>> dict = new();

		void CreateBuildNodeTreeViewElement(LogicBuildStepNode node)
		{
			if (dict.ContainsKey(node.Index) == false)
				dict.Add(node.Index, new TreeViewItemData<BuildNodeVEDataHolder>(node.Index, new BuildNodeVEDataHolder(node)));
			foreach (var subNode in node.SubSteps)
			{
				CreateBuildNodeTreeViewElement(subNode);
			}
			var treeViewSubItems = node.SubSteps.Select(subNode => dict[subNode.Index]).ToList();
			var item = dict[node.Index].data;
			// Update The Item With the Childrens
			dict[node.Index] = new TreeViewItemData<BuildNodeVEDataHolder>(node.Index, item, treeViewSubItems);
		}

		CreateBuildNodeTreeViewElement(root);
		treeViewItemDatas.Add(dict[0]);
		return treeViewItemDatas;
	}
	public List<TreeViewItemData<BuildNodeVEDataHolder>> CreateTreeViewItemsList()
	{
		return CreateTreeViewItemsList(logicBuildStepRoot);
	}

}
public class LogicBuildStepNode
{
	public string StepName;
	public int depth;
	public BuildStepMessage[] StepMessages;
	public List<LogicBuildStepNode> SubSteps;
	public int Index;
	public LogicBuildStepNode(BuildStep step, int index)
	{
		StepName = step.name;
		StepMessages = step.messages;
		depth = step.depth;
		SubSteps = new List<LogicBuildStepNode>();
		Index = index;
	}
	public BuildStepMessage[] AllChildrenMesseges()
	{
		List<BuildStepMessage> allMesseges = new List<BuildStepMessage>();
		void DFS(LogicBuildStepNode node)
		{
			if(node != this)
				allMesseges.AddRange(node.StepMessages);
			foreach (var subNode in node.SubSteps)
				DFS(subNode);
		}
		DFS(this);
		return allMesseges.ToArray();
	}
	public LogType GetWorseLogTypeInMeAndChildrens() {
		LogType worse = LogType.Log;
		void DFS(LogicBuildStepNode node)
		{
			if (node.StepMessages.Length > 0)
			{
				var worestLogInChildren = node.StepMessages.Select(m => m.type).Aggregate(WorseLogType);
				worse = WorseLogType(worse, worestLogInChildren);
			}
			foreach (var subNode in node.SubSteps)
				DFS(subNode);
		}
		DFS(this);
		return worse;
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
	override public string ToString()
		{
			string result = StepName;
			foreach (var subStep in SubSteps)
			{
				result += "\n" + new string(' ', depth) + subStep.ToString();
			}
			return result;
		}
}
public class BuildNodeVEDataHolder
{
	public string StepName;
	public Texture2D Icon;
	public LogType WorseLogType;
	public BuildStepMessage[] buildStepMessages;
	public BuildStepMessage[] childsMesseges;
	public int Index;
	public BuildNodeVEDataHolder(LogicBuildStepNode logicNode)
	{
		StepName = logicNode.StepName;
		buildStepMessages = logicNode.StepMessages;
		Index = logicNode.Index;
		WorseLogType = logicNode.GetWorseLogTypeInMeAndChildrens();
		childsMesseges = logicNode.AllChildrenMesseges();
		switch (WorseLogType)
		{
			case LogType.Error:
				Icon = IconsLibrary.Instance.Core.GetIcon("Error");
				break;
			case LogType.Warning:
				Icon = IconsLibrary.Instance.Core.GetIcon("Warning");
				break;
			default:
				Icon = IconsLibrary.Instance.Core.GetIcon("Success");
				break;
		}
	}
}
