using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PROJECT.Scripts.Editor
{
    /// <summary>
    /// Editor window for generating and editing enums in Unity
    /// </summary>
    public class EnumGeneratorWindow : EditorWindow
    {
        #region Variables
        /// <summary>
        /// The name of the enum to be generated or edited
        /// </summary>
        private string _enumName = "TypeOfEnvironmentCharacteristic";

        /// <summary>
        /// The folder path where the enum file will be saved
        /// </summary>
        private string _folderPath = "Assets/Scripts/Generated";

        /// <summary>
        /// The namespace in which the enum will be generated
        /// </summary>
        private string _namespace = "PROJECT.Scripts.Enums";
        
        /// <summary>
        /// List that contains all the enum elements/values
        /// </summary>
        private readonly List<string> _elements = new() { "Default", "Null" };

        /// <summary>
        /// Vector2 to track scroll position in the element list UI
        /// </summary>
        private Vector2 _scrollPos;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Opens the Enum Generator window in the Unity Editor
        /// </summary>
        [MenuItem("Tools/Project/Enum Generator")] 
        public static void OpenWindow() => GetWindow<EnumGeneratorWindow>("Enum Generator");

        /// <summary>
        /// Renders the editor window GUI elements
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("🧱 Enum Generator and Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);
        
            if (GUILayout.Button("📂 Load Existing Enum...", GUILayout.Height(25)))
                LoadExistingEnum();

            GUILayout.Space(10);
            _enumName = EditorGUILayout.TextField("Enum Name", _enumName);
        
            _namespace = EditorGUILayout.TextField("Namespace (optional)", _namespace);
            
            DrawFolderSelector();
            DrawElementsList();

            GUI.enabled = !string.IsNullOrEmpty(_enumName) && _elements.Count > 0;
            if (GUILayout.Button("📜 Generate Enum", GUILayout.Height(30)))
                GenerateEnumFile();
            GUI.enabled = true;
        }
        #endregion

        #region GUI Helpers
        /// <summary>
        /// Renders the UI for selecting the output folder
        /// </summary>
        private void DrawFolderSelector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Output Folder");
            EditorGUILayout.SelectableLabel(_folderPath, GUILayout.Height(16));
            if (GUILayout.Button("Select...", GUILayout.Width(100)))
                SelectOutputFolder();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Opens a folder browser dialog and validates the selected output path
        /// </summary>
        private void SelectOutputFolder()
        {
            string selected = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
            if (string.IsNullOrEmpty(selected)) return;

            if (selected.StartsWith(Application.dataPath))
                _folderPath = "Assets" + selected.Substring(Application.dataPath.Length);
            else
                EditorUtility.DisplayDialog("Warning", "Please select a folder inside 'Assets'!", "OK");
        }

        /// <summary>
        /// Renders the list of enum elements with controls to add/remove elements
        /// </summary>
        private void DrawElementsList()
        {
            GUILayout.Space(10);
            GUILayout.Label("Enum Elements:", EditorStyles.boldLabel);
        
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(150));
            for (int i = 0; i < _elements.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _elements[i] = EditorGUILayout.TextField(_elements[i]);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    _elements.RemoveAt(i--);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add Element"))
                _elements.Add("NewElement");

            GUILayout.Space(15);
        }
        #endregion

        #region File Generation
        /// <summary>
        /// Creates the enum file with the specified settings and elements
        /// </summary>
        private void GenerateEnumFile()
        {
            if (string.IsNullOrEmpty(_enumName) || _elements.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", 
                    string.IsNullOrEmpty(_enumName) ? "Enum name cannot be empty." : "Elements list cannot be empty.", "OK");
                return;
            }

            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            string filePath = Path.Combine(_folderPath, $"{_enumName}.cs");

            using var writer = new StreamWriter(filePath);
            writer.WriteLine("// Automatically generated by EnumGeneratorWindow");
            
            if (!string.IsNullOrWhiteSpace(_namespace))
            {
                writer.WriteLine($"namespace {_namespace}");
                writer.WriteLine("{");
            }
            
            writer.WriteLine($" public enum {_enumName}");
            writer.WriteLine("  {");
            foreach (string element in _elements.FindAll(e => !string.IsNullOrWhiteSpace(e)))
                writer.WriteLine($"     {MakeSafeIdentifier(element)},");
            writer.WriteLine("  }");

            if (!string.IsNullOrWhiteSpace(_namespace))
                writer.WriteLine("}");

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("✅ Success", $"Enum '{_enumName}' generated or updated at:\n{filePath}", "OK");
        }
        #endregion

        #region File Operations
        /// <summary>
        /// Loads and parses an existing enum file into the editor
        /// </summary>
        private void LoadExistingEnum()
        {
            string path = EditorUtility.OpenFilePanel("Select enum file", Application.dataPath, "cs");
            if (string.IsNullOrEmpty(path)) return;

            string content = File.ReadAllText(path);
            string relative = path.Replace(Application.dataPath, "Assets");

            var namespaceMatch = Regex.Match(content, @"namespace\s+([\w\.]+)");
            _namespace = namespaceMatch.Success ? namespaceMatch.Groups[1].Value : "";
            
            var nameMatch = Regex.Match(content, @"public\s+enum\s+(\w+)");
            if (nameMatch.Success)
                _enumName = nameMatch.Groups[1].Value;
        
            var bodyMatch = Regex.Match(content, @"{([^}]*)}");
            if (bodyMatch.Success)
            {
                _elements.Clear();
                string body = bodyMatch.Groups[1].Value;
                foreach (var line in body.Split(new[] { '\n', '\r', ',' }, System.StringSplitOptions.RemoveEmptyEntries))
                {
                    string clean = line.Trim();
                    if (!string.IsNullOrEmpty(clean) && !clean.StartsWith("//"))
                        _elements.Add(clean);
                }
            }

            _folderPath = Path.GetDirectoryName(relative)?.Replace("\\", "/");
            EditorUtility.DisplayDialog("✅ Enum Loaded", $"Enum '{_enumName}' loaded successfully!", "OK");
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Creates a valid C# identifier by removing/replacing invalid characters
        /// </summary>
        /// <param name="name">The input string to sanitize</param>
        /// <returns>A valid C# identifier string</returns>
        private static string MakeSafeIdentifier(string name)
        {
            string safe = name.Trim().Replace(" ", "_").Replace("-", "_");
            return safe.Length > 0 && char.IsDigit(safe[0]) ? "_" + safe : safe;
        }
        #endregion
    }
}