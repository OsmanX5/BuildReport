using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TestWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/TestWindow")]
    public static void ShowExample()
    {
        TestWindow wnd = GetWindow<TestWindow>();
        wnd.titleContent = new GUIContent("TestWindow");
    }
	ObjectField m_ObjectField;
	VisualElement CardHolder;

	public void CreateGUI()
    {
		Debug.Log(AssetDatabase.GetAssetPath(m_VisualTreeAsset));
        Debug.Log("CreateGUI");
		m_VisualTreeAsset.CloneTree(rootVisualElement);
		CardHolder = rootVisualElement.Q<VisualElement>("CardHolder");
		m_ObjectField = rootVisualElement.Q<ObjectField>("ObjectField");
        m_ObjectField.RegisterValueChangedCallback(OnObjectFieldChanged);

	}

	private void OnObjectFieldChanged(ChangeEvent<UnityEngine.Object> evt)
	{
		Debug.Log("OnObjectFieldChanged");
		CardHolder.Clear();
		SelectedAssetCardVE selectedAssetCardVE = new SelectedAssetCardVE();
		CardHolder.Add(selectedAssetCardVE);
	}
}
