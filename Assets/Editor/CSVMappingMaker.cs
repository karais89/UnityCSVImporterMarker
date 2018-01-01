/*
 * CSV 파일을 읽어와서 클래스로 매핑 함.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CSVMappingMaker : EditorWindow
{
    private enum ValueType
    {
        Bool,
        String,
        Int,
        Float,
        Double
    };

    private class RowParam
    {
        public ValueType type;
        public string name;
    };
    
    private static readonly string KEY_PREFS = "CSVMappingMaker_"; // 윈도우 정보 임시 저장
    private string _filePath; // csv 파일 경로
    private string _fileName; // csv 파일 이름
    private string _className; // csv 파일을 매핑할 클래스 이름
    private List<RowParam> _rowParams = new List<RowParam>();
    private Vector2 _scrollPosition;
    
    [MenuItem("Assets/CSV Mapping To Class")]
    public static void CSVMappingToClass()
    {
        // 선택된 에셋을 처리 한다.
        foreach (var obj in Selection.objects)
        {
            if (obj == null)
            {
                continue;
            }
            
            // 무조건 새로운 윈도우를 생성 해준다.
            var window = CreateInstance<CSVMappingMaker>();
            window._filePath = AssetDatabase.GetAssetPath(obj);
            window._fileName = Path.GetFileNameWithoutExtension(window._filePath);
            
            // 해당 파일 경로의 텍스트를 읽어온다.
            string csv = File.ReadAllText(window._filePath);
            
            // 해당 파일이 있으면 이전 정보를 세팅 해주고, 없으면 기본 값을 세팅 해준다.
            window._className = EditorPrefs.GetString(GetKeyPrefsByClassName(window), window._fileName + "CSVData");
            
            // csv 파싱
            var data = SimpleCSVParser.Parser(csv);
            if (data.Count < 2)
            {
                Debug.LogError("최소한 헤더 정보와 값 정보를 가지고 있어야 합니다. (row가 2줄 이상)");
                continue;
            }
            
            var headerMeta = data[0]; // 자료형 이름 판단
            var rowMeta = data[1]; // 자료형 타입 판단
            window._rowParams.Clear();
            for (int i = 0; i < headerMeta.Count; i++)
            {
                RowParam param = new RowParam();
         
                param.name = headerMeta[i];

                if (EditorPrefs.HasKey(GetKeyPrefsByType(window, param)))
                {
                    param.type = (ValueType)EditorPrefs.GetInt(GetKeyPrefsByType(window, param));
                }
                else
                {
                    param.type = GetValueTypeByRowMeta(rowMeta[i]);
                }
                
                window._rowParams.Add(param);
            }
            window.Show();
        }
    }

    private static string GetKeyPrefsByType(CSVMappingMaker window, RowParam param)
    {
        return KEY_PREFS + window._fileName + ".type." + param.name;
    }

    private static string GetKeyPrefsByClassName(CSVMappingMaker window)
    {
        return KEY_PREFS + window._fileName + ".className";
    }

    /// <summary>
    /// string 자료형 데이터로 데이터 타입을 판단해서 반환한다.
    /// 실수형의 경우는 무조건 float형을 반환하며, double형을 사용하고 싶을 경우네는 사용자가 EditorWindow상에서 변경 해준다.
    /// 자료형의 변환 우선 순위는 int -> float -> bool -> string 순이다.
    /// </summary>
    /// <param name="rowData"></param>
    /// <returns></returns>
    private static ValueType GetValueTypeByRowMeta(string rowData)
    {
        int intValue;
        float floatValue;
        bool boolValue;
        
        if (int.TryParse(rowData, out intValue))
        {
            return ValueType.Int;
        }
        
        if (float.TryParse(rowData, out floatValue))
        {
            return ValueType.Float;
        }
        
        if (Boolean.TryParse(rowData, out boolValue))
        {
            return ValueType.Bool;
        }
        
        return ValueType.String;
    }

    private void OnGUI()
    {
        var window = GetWindow<CSVMappingMaker>();
        
        GUILayout.Label("CSV Mapping To Class", EditorStyles.boldLabel);
        _className = EditorGUILayout.TextField("class Name", _className);
        
        if (GUILayout.Button("Csv Data Mapping!!"))
        {
            EditorPrefs.SetString(GetKeyPrefsByClassName(window), _className);
            // todo : 파일 생성 및 에셋 업데이트
            
            AssetDatabase.ImportAsset(_filePath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }
        
        // csv header settings
        EditorGUILayout.LabelField("csv header settings");
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        EditorGUILayout.BeginVertical("box");

        foreach (var row in _rowParams)
        {
            GUILayout.BeginHorizontal();

            row.name = EditorGUILayout.TextField(row.name);
            row.type = (ValueType) EditorGUILayout.EnumPopup(row.type, GUILayout.MaxWidth(100));
            EditorPrefs.SetInt(GetKeyPrefsByType(window, row), (int)row.type);
            
            GUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}