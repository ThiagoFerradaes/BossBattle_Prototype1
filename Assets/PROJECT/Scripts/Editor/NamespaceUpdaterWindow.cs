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
            public bool autoCreateAsmdefs = true;
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
            window.minSize = new Vector2(850, 450);
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
            EditorGUILayout.HelpBox("Automatically updates script namespaces and asmdefs based on folder structure.", MessageType.Info);
            EditorGUILayout.Space(10);
        }

        private void DrawSettings()
        {
            EditorGUILayout.LabelField("⚙ Settings", EditorStyles.boldLabel);

            DrawFolderSelector();
            DrawEditorScriptsToggle();
            DrawAsmdefToggle();
            DrawIgnoredFolders();
        }

        private void DrawFolderSelector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base folder:", GUILayout.Width(90));
            EditorGUILayout.SelectableLabel(_filterPath, EditorStyles.textField, GUILayout.Height(18));
            if (GUILayout.Button("Select folder", GUILayout.Width(130)))
                SelectBaseFolder();
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

        private void DrawAsmdefToggle()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            _settings.autoCreateAsmdefs = EditorGUILayout.Toggle(_settings.autoCreateAsmdefs, GUILayout.Width(20));
            EditorGUILayout.LabelField("Auto-create & sync Assembly Definitions (.asmdef)", GUILayout.ExpandWidth(true));
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

            // Remove "Assets/" e transforma o restante em formato de namespace
            string relativePath = path
                .Replace("Assets/", "")
                .TrimStart('/')
                .Replace("/", ".");

            // 🔹 Se quiser SEMPRE basear apenas na pasta, ignora o rootNamespace completamente
            string newNs = relativePath;

            // 🔸 Se estiver na raiz "Assets/" (sem subpastas), evita namespace vazio
            if (string.IsNullOrEmpty(newNs))
                newNs = "Global"; // ou outro nome padrão, tipo "Root"

            return (currentNs, newNs);
            
        }

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

                    if (_settings.autoCreateAsmdefs)
                    {
                        string folder = Path.GetDirectoryName(item.Path);
                        if (!string.IsNullOrEmpty(folder))
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
        /// Ensures the folder has a matching Assembly Definition (.asmdef).
        /// </summary>
        private void EnsureAssemblyDefinition(string directoryPath, string namespaceName)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                return;

            string[] asmdefFiles = Directory.GetFiles(directoryPath, "*.asmdef", SearchOption.TopDirectoryOnly);
            string targetAsmdefPath = Path.Combine(directoryPath, $"{namespaceName}.asmdef");

            // Detect dependencies before creating/updating
            List<string> detectedDependencies = DetectAsmdefDependencies(directoryPath);

            if (asmdefFiles.Length == 0)
            {
                var json = "{\n" +
                           $"  \"name\": \"{namespaceName}\",\n" +
                           $"  \"rootNamespace\": \"{namespaceName}\",\n" +
                           $"  \"references\": [\n{string.Join(",\n", detectedDependencies.Select(d => $"    \"{d}\""))}\n  ]\n" +
                           "}";
                File.WriteAllText(targetAsmdefPath, json);
                Debug.Log($"Created new asmdef: {targetAsmdefPath}");
                return;
            }

            string existingPath = asmdefFiles[0];
            string content = File.ReadAllText(existingPath);
            var match = Regex.Match(content, "\"name\"\\s*:\\s*\"([^\"]+)\"");
            if (!match.Success) return;

            string existingName = match.Groups[1].Value;
            bool changed = false;

            // Update name/rootNamespace if needed
            if (existingName != namespaceName)
            {
                content = Regex.Replace(content, "\"name\"\\s*:\\s*\"([^\"]+)\"", $"\"name\": \"{namespaceName}\"");
                changed = true;
            }

            if (content.Contains("\"rootNamespace\""))
                content = Regex.Replace(content, "\"rootNamespace\"\\s*:\\s*\"([^\"]+)\"",
                    $"\"rootNamespace\": \"{namespaceName}\"");
            else
                content = content.TrimEnd('}', '\n', '\r', ' ') + $",\n  \"rootNamespace\": \"{namespaceName}\"\n}}";

            // Update references section
            string refsJson = string.Join(",\n", detectedDependencies.Select(d => $"    \"{d}\""));
            if (Regex.IsMatch(content, "\"references\"\\s*:\\s*\\[[^\\]]*\\]"))
                content = Regex.Replace(content, "\"references\"\\s*:\\s*\\[[^\\]]*\\]",
                    $"\"references\": [\n{refsJson}\n  ]");
            else
                content = content.TrimEnd('}', '\n', '\r', ' ') + $",\n  \"references\": [\n{refsJson}\n  ]\n}}";

            if (changed || detectedDependencies.Count > 0)
                File.WriteAllText(existingPath, content);

            // Rename file if name mismatch
            if (!existingPath.EndsWith($"{namespaceName}.asmdef"))
            {
                string newPath = Path.Combine(directoryPath, $"{namespaceName}.asmdef");
                File.Move(existingPath, newPath);
                Debug.Log($"Renamed asmdef: {existingPath} → {newPath}");
            }
        }

        /// <summary>
        /// Scans scripts in the folder and detects which namespaces are used,
        /// then matches them to existing asmdefs to infer dependencies.
        /// </summary>
        private List<string> DetectAsmdefDependencies(string folderPath)
        {
            var dependencies = new HashSet<string>();
            var allAsmdefs = Directory.GetFiles("Assets", "*.asmdef", SearchOption.AllDirectories);

            // Map asmdef name -> rootNamespace
            var asmdefMap = new Dictionary<string, string>();
            foreach (var asmdef in allAsmdefs)
            {
                string json = File.ReadAllText(asmdef);
                string name = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
                string rootNs = Regex.Match(json, "\"rootNamespace\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
                if (!string.IsNullOrEmpty(name))
                    asmdefMap[rootNs] = name;
            }

            // Analyze usings in current folder scripts
            foreach (var cs in Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories))
            {
                foreach (Match m in Regex.Matches(File.ReadAllText(cs), @"using\s+([A-Za-z0-9_.]+)\s*;"))
                {
                    string usedNs = m.Groups[1].Value;
                    if (asmdefMap.TryGetValue(usedNs, out var asmName))
                        dependencies.Add(asmName);
                }
            }

            return dependencies.ToList();
        }
        
        private static string DetectRootNamespace()
        {
            // Busca subpastas diretas dentro de Assets
            var subDirs = Directory.GetDirectories("Assets");

            // Caso só exista uma pasta, usa o nome dela
            if (subDirs.Length == 1)
                return Path.GetFileName(subDirs[0]);

            // Caso exista uma pasta com nome similar ao projeto
            string projectName = Application.productName;
            string match = subDirs.FirstOrDefault(d =>
                Path.GetFileName(d).ToLower().Contains(projectName.ToLower()));

            // Se encontrou, usa o nome da pasta
            if (!string.IsNullOrEmpty(match))
                return Path.GetFileName(match);

            // 🔧 Fallback: usa o nome do projeto (garante que nunca fique vazio)
            if (!string.IsNullOrEmpty(projectName))
                return projectName.Replace(" ", "_");

            // Último fallback caso Application.productName esteja vazio
            return "Project";
        }

        #endregion
    }
}