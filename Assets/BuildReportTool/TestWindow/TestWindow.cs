using UnityEditor;
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

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        root.Add(new SelectedAssetCardVE());
    }
}
