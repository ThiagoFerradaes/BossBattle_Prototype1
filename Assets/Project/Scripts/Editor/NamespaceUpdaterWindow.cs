using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PROJECT.Scripts.Editor
{
    /// <summary>
    /// Editor window tool for automatically updating namespaces in C# scripts based on folder structure.
    /// </summary>
    public class NamespaceUpdaterWindow : EditorWindow
    {
        #region Internal Types

        /// <summary>
        /// Settings class that stores namespace updater configurations.
        /// </summary>
        [Serializable]
        public class NamespaceUpdaterSettings : ScriptableObject
        {
            public bool includeEditorScripts;
            public List<string> ignoredFolders = new() { "Plugins", "ThirdParty", "External", "Generated" };

            private const string AssetPath = "Assets/Editor/NamespaceUpdaterSettings.asset";

            /// <summary>
            /// Loads existing settings or creates new ones if not found.
            /// </summary>
            public static NamespaceUpdaterSettings LoadOrCreate()
            {
                var settings = AssetDatabase.LoadAssetAtPath<NamespaceUpdaterSettings>(AssetPath);
                if (settings != null) return settings;
                
                settings = CreateInstance<NamespaceUpdaterSettings>();
                Directory.CreateDirectory(Path.GetDirectoryName(AssetPath)!);
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
                return settings;
            }
        }

        /// <summary>
        /// Structure for displaying namespace changes preview.
        /// </summary>
        private readonly struct NamespacePreview
        {
            public readonly string Path;
            public readonly string CurrentNamespace; 
            public readonly string NewNamespace;

            public NamespacePreview(string path, string currentNamespace, string newNamespace)
            {
                Path = path;
                CurrentNamespace = currentNamespace;
                NewNamespace = newNamespace;
            }
        }

        #endregion

        #region Variables

        private NamespaceUpdaterSettings _settings;
        private string _filterPath = "Assets/";
        private readonly List<NamespacePreview> _previewList = new();
        private Vector2 _scroll;

        #endregion

        #region Menu Item

        [MenuItem("Tools/Project/Namespace Updater")]
        public static void ShowWindow()
        {
            var window = GetWindow<NamespaceUpdaterWindow>("Namespace Updater");
            window.minSize = new Vector2(800, 400);
        }

        #endregion

        #region Unity GUI

        private void OnGUI()
        {
            _settings ??= NamespaceUpdaterSettings.LoadOrCreate();

            DrawHeader();
            DrawSettings();
            DrawActionButtons();
            DrawPreviewList();
            
            // Auto-save settings
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(" Namespace Updater Tool", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Automatically updates script namespaces based on folder structure.",
                MessageType.Info);
            EditorGUILayout.Space(10);
        }

        private void DrawSettings()
        {
            EditorGUILayout.LabelField("⚙ Settings", EditorStyles.boldLabel);

            DrawFolderSelector();
            DrawEditorScriptsToggle();
            DrawIgnoredFolders();
        }

        private void DrawFolderSelector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base folder:", GUILayout.Width(90));
            EditorGUILayout.SelectableLabel(_filterPath, EditorStyles.textField, GUILayout.Height(18));
            if (GUILayout.Button("Select folder", GUILayout.Width(130)))
            {
                SelectBaseFolder();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SelectBaseFolder()
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select base folder", "Assets", "");
            if (string.IsNullOrEmpty(selectedPath)) return;

            if (selectedPath.Contains(Application.dataPath))
                _filterPath = "Assets" + selectedPath.Replace(Application.dataPath, "").Replace("\\", "/");
            else
                EditorUtility.DisplayDialog("Warning", "Selected folder must be inside 'Assets/'.", "OK");
        }

        private void DrawEditorScriptsToggle()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            _settings.includeEditorScripts = EditorGUILayout.Toggle(_settings.includeEditorScripts, GUILayout.Width(20));
            EditorGUILayout.LabelField("Include scripts in 'Editor' folders", GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawIgnoredFolders()
        {
            EditorGUILayout.LabelField("Ignored folders:");
            for (int i = 0; i < _settings.ignoredFolders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _settings.ignoredFolders[i] = EditorGUILayout.TextField(_settings.ignoredFolders[i]);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                    _settings.ignoredFolders.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add ignored folder"))
                _settings.ignoredFolders.Add("NewFolder");
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(" Scan Project", GUILayout.Height(30)))
                ScanProject();
            if (GUILayout.Button(" Apply Namespaces", GUILayout.Height(30)))
                ApplyChanges();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPreviewList()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(" Changes Preview", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var item in _previewList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Path.GetFileName(item.Path), GUILayout.Width(200));
                EditorGUILayout.LabelField(item.CurrentNamespace, EditorStyles.miniLabel, GUILayout.Width(250));
                EditorGUILayout.LabelField("→", GUILayout.Width(20));
                EditorGUILayout.LabelField(item.NewNamespace, EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Click 'Apply Namespaces' to update all listed files.", MessageType.None);
        }

        #endregion

        #region Core Logic

        /// <summary>
        /// Ensures that the folder containing the file has a matching Assembly Definition (.asmdef)
        /// with the same name as the namespace.
        /// </summary>
        private void EnsureAssemblyDefinition(string directoryPath, string namespaceName)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                return;

            string[] asmdefFiles = Directory.GetFiles(directoryPath, "*.asmdef", SearchOption.TopDirectoryOnly);

            // Desired path for new asmdef
            string targetAsmdefPath = Path.Combine(directoryPath, $"{namespaceName}.asmdef");

            if (asmdefFiles.Length == 0)
            {
                // Create new asmdef
                var json = "{\n" +
                           $"  \"name\": \"{namespaceName}\",\n" +
                           "  \"rootNamespace\": \"" + namespaceName + "\"\n" +
                           "}";
                File.WriteAllText(targetAsmdefPath, json);
                Debug.Log($"Created new asmdef: {targetAsmdefPath}");
                return;
            }

            // If there's an existing asmdef, check its name
            string existingPath = asmdefFiles[0];
            string content = File.ReadAllText(existingPath);

            var match = Regex.Match(content, "\"name\"\\s*:\\s*\"([^\"]+)\"");
            if (match.Success)
            {
                string existingName = match.Groups[1].Value;
                if (existingName != namespaceName)
                {
                    // Update name and rootNamespace
                    content = Regex.Replace(content, "\"name\"\\s*:\\s*\"([^\"]+)\"", $"\"name\": \"{namespaceName}\"");
                    if (content.Contains("\"rootNamespace\""))
                        content = Regex.Replace(content, "\"rootNamespace\"\\s*:\\s*\"([^\"]+)\"",
                            $"\"rootNamespace\": \"{namespaceName}\"");
                    else
                        content = content.TrimEnd('}', '\n', '\r', ' ') +
                                  $",\n  \"rootNamespace\": \"{namespaceName}\"\n}}";

                    File.WriteAllText(existingPath, content);

                    // Rename file if name doesn't match
                    if (!existingPath.EndsWith($"{namespaceName}.asmdef"))
                    {
                        string newPath = Path.Combine(directoryPath, $"{namespaceName}.asmdef");
                        
                        File.Move(existingPath, newPath);
                        
                        Debug.Log($"Renamed asmdef: {existingPath} → {newPath}");
                    }
                }
            }
        }


        /// <summary>
        /// Scans project for files needing namespace updates.
        /// </summary>
        private void ScanProject()
        {
            _previewList.Clear();

            string rootNamespace = DetectRootNamespace();
            var files = Directory.GetFiles(_filterPath, "*.cs", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (!ShouldProcessFile(file)) continue;

                var (currentNs, newNs) = GetNamespaceInfo(file, rootNamespace);
                if (string.IsNullOrEmpty(currentNs) || currentNs != newNs)
                {
                    _previewList.Add(new NamespacePreview(
                        file.Replace("\\", "/"),
                        string.IsNullOrEmpty(currentNs) ? "(none)" : currentNs,
                        newNs));
                }
            }

            Debug.Log($" Found {_previewList.Count} files to update namespaces.");
        }

        private bool ShouldProcessFile(string file)
        {
            string normalizedPath = file.Replace("\\", "/");
            if (!_settings.includeEditorScripts && normalizedPath.Contains("/Editor/"))
                return false;

            return !_settings.ignoredFolders.Any(f => normalizedPath.Contains($"/{f}/"));
        }

        private (string currentNs, string newNs) GetNamespaceInfo(string file, string rootNamespace)
        {
            string content = File.ReadAllText(file);
            string currentNs = Regex.Match(content, @"namespace\s+([a-zA-Z0-9_.]+)").Groups[1].Value;

            string path = Path.GetDirectoryName(file)?.Replace("\\", "/") ?? "";
            string relativePath = path.Replace("Assets/", "").Replace("/", ".");
            string newNs = $"{rootNamespace}.{relativePath}".Replace("..", ".");

            return (currentNs, newNs);
        }

        /// <summary>
        /// Applies namespace changes to all scanned files.
        /// </summary>
        private void ApplyChanges()
        {
            if (_previewList.Count == 0)
            {
                Debug.LogWarning("No files to update. Run 'Scan Project' first.");
                return;
            }

            string backupDir = "Assets/Editor/NamespaceBackups";
            Directory.CreateDirectory(backupDir);

            foreach (var item in _previewList)
            {
                try
                {
                    UpdateFile(item, backupDir);
                    
                    string folder = Path.GetDirectoryName(item.Path);
                    if (!string.IsNullOrEmpty(folder))
                    {
                        EnsureAssemblyDefinition(folder, item.NewNamespace);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error updating {item.Path}: {ex.Message}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log($" Successfully updated {_previewList.Count} files!");
        }

        private static void UpdateFile(NamespacePreview item, string backupDir)
        {
            string backupFile = Path.Combine(backupDir, Path.GetFileName(item.Path));
            File.Copy(item.Path, backupFile, true);

            string content = File.ReadAllText(item.Path);
            content = Regex.IsMatch(content, @"namespace\s+[a-zA-Z0-9_.]+")
                ? Regex.Replace(content, @"namespace\s+[a-zA-Z0-9_.]+", $"namespace {item.NewNamespace}")
                : $"namespace {item.NewNamespace}\n{{\n{content}\n}}";

            File.WriteAllText(item.Path, content);
        }

        /// <summary>
        /// Detects root namespace based on the project structure.
        /// </summary>
        private static string DetectRootNamespace()
        {
            var subDirs = Directory.GetDirectories("Assets");
            if (subDirs.Length == 1)
                return Path.GetFileName(subDirs[0]);

            string projectName = Application.productName;
            string match = subDirs.FirstOrDefault(d => 
                Path.GetFileName(d).ToLower().Contains(projectName.ToLower()));
            
            return !string.IsNullOrEmpty(match) ? Path.GetFileName(match) : "Assets";
        }

        #endregion
    }
}