/*
 * CSV를 단지 보기 위해 만든 툴
 * 의미는 크게 없으며, 단지 테스트 용도.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CSVViewer : EditorWindow
{
    private TextAsset asset;
    private List<List<string>> allLines;
    private Vector2 scrollPosition;
    
    [MenuItem("Window/CSV Viewer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<CSVViewer>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        TextAsset newAsset = EditorGUILayout.ObjectField("CSV File", asset, typeof(TextAsset), false) as TextAsset;
        EditorGUILayout.EndHorizontal();

        if (newAsset == null)
        {
            return;
        }

        if (newAsset != asset)
        {
            asset = newAsset;
            allLines = SimpleCSVParser.Parser(asset.text);
        }

        if (GUILayout.Button("Refresh!"))
        {
            allLines = SimpleCSVParser.Parser(asset.text);
        }
        
        // 에디터 윈도우 크기를 넘어가면 스크롤 되게 한다.
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));
        foreach (var line in allLines)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var word in line)
            {
                EditorGUILayout.TextField(word);
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}