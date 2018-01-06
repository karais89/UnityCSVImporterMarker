namespace Karais.CSV
{
    /*
     * CSV 파일을 읽어와서 클래스로 매핑 함.
     */    
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
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
            public bool isArray; // 데이터가 배열형인가? 쌍 따옴표 안에 콤마로 구분 된 데이터 "1,2,3,4,5"
            public ValueType type; // 데이터 타입
            public string name; // 키값
            public string firstRowValue; // 첫번째 열 데이터 값 저장 (보여주는 용도)
        };
        
        private static readonly string BASE_CLASS_CREATE_PATH = "Assets/Scripts/CSVData/"; // 초기 csvData 클래스가 생성되는 곳
        private static readonly string KEY_PREFS = "CSVMappingMaker_"; // 윈도우 정보 임시 저장
        
        private string _filePath; // csv 파일 경로
        private string _fileName; // csv 파일 이름
        private string _className; // csv 파일을 매핑할 클래스 이름
        private string _createClassPath; // 클래스 파일이 생성되는 경로
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
    
                if (Path.GetExtension(window._filePath).ToLower() != ".csv")
                {
                    Debug.LogError("This is file not csv format");
                    continue;
                }
    
                // 해당 파일 경로의 텍스트를 읽어온다.
                string csv = File.ReadAllText(window._filePath);
    
                // 해당 파일이 있으면 이전 정보를 세팅 해주고, 없으면 기본 값을 세팅 해준다.
                window._createClassPath = EditorPrefs.GetString(GetKeyPrefsByCreateClassPath(), BASE_CLASS_CREATE_PATH);
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
                    param.firstRowValue = rowMeta[i];
    
                    CheckArray(window, param, rowMeta[i]);
                    CheckValue(window, param, rowMeta[i]);

                    window._rowParams.Add(param);
                }
    
                window.Show();
            }
        }

        private static void CheckValue(CSVMappingMaker window, RowParam param, string rowMeta)
        {
            // value type 체크
            if (EditorPrefs.HasKey(GetKeyPrefsByType(window, param)))
            {
                param.type = (ValueType) EditorPrefs.GetInt(GetKeyPrefsByType(window, param));
            }
            else
            {
                string rowData = rowMeta;
                if (param.isArray)
                {
                    rowData = rowData.Split(',')[0];
                }
                
                param.type = GetValueTypeByRowMeta(rowData);
            }
        }

        private static void CheckArray(CSVMappingMaker window, RowParam param, string rowMeta)
        {
            // array 체크
            if (EditorPrefs.HasKey(GetKeyPrefsByIsArray(window, param)))
            {
                param.isArray = EditorPrefs.GetInt(GetKeyPrefsByIsArray(window, param)) != 0;
            }
            else
            {
                param.isArray = IsArrayByRowMeta(rowMeta);
            }
        }

        private static string GetKeyPrefsByType(CSVMappingMaker window, RowParam param)
        {
            return KEY_PREFS + window._fileName + ".type." + param.name;
        }
        
        private static string GetKeyPrefsByIsArray(CSVMappingMaker window, RowParam param)
        {
            return KEY_PREFS + window._fileName + ".isArray." + param.name;
        }
    
        private static string GetKeyPrefsByClassName(CSVMappingMaker window)
        {
            return KEY_PREFS + window._fileName + ".className";
        }
    
        private static string GetKeyPrefsByCreateClassPath()
        {
            return KEY_PREFS + ".createClassPath";
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
    
            if (bool.TryParse(rowData, out boolValue))
            {
                return ValueType.Bool;
            }
    
            return ValueType.String;
        }

        /// <summary>
        /// 해당 자료형이 배열인가의 판단 여부
        /// SimpleParser의 경우 쌍 따옴표안에 콤마의 경우 쌍 따옴표를 제거해주고 그대로 값에 대응해준다.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private static bool IsArrayByRowMeta(string rowData)
        {
            return rowData.Split(',').Length > 1;
        }
    
        private void OnGUI()
        {
            var window = GetWindow<CSVMappingMaker>();
    
            GUILayout.Label("CSV Mapping To Class", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            _createClassPath = EditorGUILayout.TextField("class Path", _createClassPath);
            if (GUILayout.Button("Reset", GUILayout.Width(50)))
            {
                _createClassPath = BASE_CLASS_CREATE_PATH;
                EditorPrefs.SetString(GetKeyPrefsByCreateClassPath(), _createClassPath);
            }
            GUILayout.EndHorizontal();
            
            _className = EditorGUILayout.TextField("class Name", _className);
    
            if (GUILayout.Button("Csv Data Mapping!!"))
            {
                EditorPrefs.SetString(GetKeyPrefsByCreateClassPath(), _createClassPath);
                EditorPrefs.SetString(GetKeyPrefsByClassName(window), _className);
    
                Debug.Log(_createClassPath);
                
                // 파일 생성 및 에셋 업데이트
                CreateFileCsvMapping();
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
                row.isArray = EditorGUILayout.ToggleLeft("data type array?", row.isArray);
                if (row.isArray)
                {
                    EditorGUILayout.LabelField("---[array]--- ex) \"1,2,3,4,5\" 쌍따옴표 안에 콤마로 데이터 구분");
                }
                
                GUILayout.BeginHorizontal();
                
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("key: ", GUILayout.MaxWidth(80));
                row.name = EditorGUILayout.TextField(row.name);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("1st row val: ", GUILayout.MaxWidth(80));
                row.firstRowValue = EditorGUILayout.TextField(row.firstRowValue);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                row.type = (ValueType) EditorGUILayout.EnumPopup(row.type, GUILayout.MaxWidth(100));
                EditorPrefs.SetInt(GetKeyPrefsByType(window, row), (int)row.type);
                EditorPrefs.SetInt(GetKeyPrefsByIsArray(window, row), row.isArray ? 1 : 0);
                GUILayout.EndHorizontal();
            }
    
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    
        private void CreateFileCsvMapping()
        {
            string templateStr = CSVMappingTemplate.codeTemplate;
            templateStr = templateStr.Replace("$CLASS_NAME$", _className);
            templateStr = templateStr.Replace("$TYPES$", GetTypesBuilder().ToString());
            templateStr = templateStr.Replace("$EXPORT_DATA$", GetExportDataBuilder().ToString());
    
            // 파일 생성
            Directory.CreateDirectory(_createClassPath);
            File.WriteAllText(Path.Combine(_createClassPath, _className + ".cs"), templateStr);
        }
    
        private StringBuilder GetTypesBuilder()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _rowParams.Count; i++)
            {
                if (i != 0)
                {
                    builder.AppendLine();
                }

                var row = _rowParams[i];
                string strType = row.type.ToString().ToLower();
                if (row.isArray)
                {
                    strType = "List<" + strType + ">";
                }
                
                builder.AppendFormat("        public {0} {1} {{ get; set; }}",
                    strType,
                    row.name);
            }
            
            return builder;
        }
    
        private StringBuilder GetExportDataBuilder()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _rowParams.Count; i++)
            {
                if (i != 0)
                {
                    builder.AppendLine();
                }
                
                string strData = string.Format("parserData[i][{0}]", i);
                string strFormat = strData;
                string strType = _rowParams[i].type.ToString().ToLower();
                switch (_rowParams[i].type)
                {
                    case ValueType.Bool:
                    case ValueType.Int:
                    case ValueType.Float:
                    case ValueType.Double:
                        strFormat = string.Format("{0}.Parse({1})", strType, strData);
                        break;
                    default:
                        break;
                }

                if (_rowParams[i].isArray)
                {
                    if (_rowParams[i].type == ValueType.String)
                    {
                        strFormat = string.Format("new List<{0}>({1}.Split(','))", strType, strData);
                    }
                    else
                    {
                        strFormat = string.Format("new List<{0}>(Array.ConvertAll({1}.Split(','), {0}.Parse))", 
                            strType, strData);   
                    }
                }
                
                builder.AppendFormat("            row.{0} = {1};", _rowParams[i].name, strFormat);   
            }
    
            return builder;
        }
    }
}