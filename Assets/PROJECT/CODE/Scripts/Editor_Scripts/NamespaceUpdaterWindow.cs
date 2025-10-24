#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NameSpaceEditor
{
    public class NamespaceUpdaterWindow : EditorWindow
    {
        private DefaultAsset _rootFolder;
        private Dictionary<string, ScriptUsingInfo> _scripts = new();
        private Vector2 _scroll;

        [MenuItem("Tools/Namespace Updater")]
        public static void OpenWindow()
        {
            GetWindow<NamespaceUpdaterWindow>("Namespace Updater");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Namespace & Using Updater", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Selecione a pasta raiz (Assets/... ) e clique em Scan Scripts.\nDepois revise e clique em Apply Changes.",
                MessageType.Info);
            EditorGUILayout.Space(4);

            _rootFolder =
                (DefaultAsset)EditorGUILayout.ObjectField("Pasta raiz", _rootFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Scan Scripts", GUILayout.Height(28)))
                {
                    Scan();
                }

                GUI.enabled = _scripts.Count > 0;
                if (GUILayout.Button("Apply Changes", GUILayout.Height(28)))
                {
                    if (EditorUtility.DisplayDialog("Confirmar",
                            $"Vai sobrescrever {_scripts.Count} arquivos. Deseja continuar?", "Sim", "Não"))
                    {
                        Apply();
                    }
                }

                GUI.enabled = true;
            }

            EditorGUILayout.Space(8);

            if (_scripts.Count > 0)
            {
                EditorGUILayout.LabelField($"Scripts encontrados: {_scripts.Count}", EditorStyles.boldLabel);
                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(300));
                foreach (var kv in _scripts.OrderBy(k => Path.GetFileName(k.Key)))
                {
                    var info = kv.Value;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(Path.GetFileName(info.ScriptPath), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Namespace:", info.Namespace);
                    EditorGUILayout.LabelField("Usings (" + info.Usings.Count + "):");
                    foreach (var u in info.Usings)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($" - {u.Name}", GUILayout.MaxWidth(400));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(u.CanBeModified ? "modificável" : "", GUILayout.MaxWidth(100));
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void Scan()
        {
            if (_rootFolder == null)
            {
                EditorUtility.DisplayDialog("Erro", "Selecione uma pasta raiz primeiro!", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(_rootFolder);
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("Erro", "Pasta inválida!", "OK");
                return;
            }

            _scripts = NamespaceUpdaterProcessor.ScanScripts(path);
        }

        private async void Apply()
        {
            if (_scripts.Count == 0)
            {
                Debug.LogWarning("[NamespaceUpdater] Nenhum script para atualizar.");
                return;
            }
            
            string path = AssetDatabase.GetAssetPath(_rootFolder);
            
            await NamespaceUpdaterProcessor.ApplyNamespaceAndUsings(_scripts, path);
            
            await NamespaceUpdaterProcessor.UpdateNamespaceRegistry(path);
            
            var cyclicDependencies = await NamespaceUpdaterProcessor.SmartCleanUsings(path ,NamespaceRegistry.LoadRegistry());
            
            if (cyclicDependencies.Count > 0)
            {
                CyclesViewerWindow.Open(cyclicDependencies);
            }
            else
            {
                EditorUtility.DisplayDialog("Ciclos", "Nenhum ciclo detectado.", "OK");
            }
        }
    }
    
    public class CyclesViewerWindow : EditorWindow
    {
        private Vector2 _scroll;
        private List<List<string>> _cycles;

        // Abrir a janela
        public static void Open(List<List<string>> cycles)
        {
            var window = GetWindow<CyclesViewerWindow>("Ciclos de Assemblies");
            window._cycles = cycles;
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Ciclos de Dependências Detectados", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            if (_cycles == null || _cycles.Count == 0)
            {
                EditorGUILayout.HelpBox("Nenhum ciclo detectado!", MessageType.Info);
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _cycles.Count; i++)
            {
                var cycle = _cycles[i];
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Ciclo #{i + 1}", EditorStyles.boldLabel);

                // Mostra os assemblies do ciclo
                for (int j = 0; j < cycle.Count; j++)
                {
                    EditorGUILayout.LabelField($" {j + 1}. {cycle[j]}");
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif