// 9/23/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualElementUtilities {
    public static VisualTreeAsset LoadUXML(string uxmlName) {
        string[] guids = AssetDatabase.FindAssets(uxmlName + " t:VisualTreeAsset");
        if (guids.Length > 0) {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        } else {
            Debug.LogError("UXML file not found for name: " + uxmlName);
            return null;
        }
    }
}
