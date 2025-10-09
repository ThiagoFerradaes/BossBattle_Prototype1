using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EnumGeneratorWindow : EditorWindow
{
    private string enumName = "TypeOfEnvironmentCharacteristic";
    private string folderPath = "Assets/Scripts/Generated";
    private List<string> elements = new List<string> { "Default", "Null" };
    private Vector2 scrollPos;
    
    [MenuItem("Tools/Enum Generator")]
    public static void OpenWindow()
    {
        GetWindow<EnumGeneratorWindow>("Enum Generator");
    }
    
     private void OnGUI()
    {
        GUILayout.Label("Gerador de Enum Dinâmico", EditorStyles.boldLabel);
        GUILayout.Space(10);

        enumName = EditorGUILayout.TextField("Nome do Enum", enumName);
        
        //folderPath = EditorGUILayout.TextField("Pasta de Saída", folderPath);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Pasta de Saída");
        EditorGUILayout.SelectableLabel(folderPath, GUILayout.Height(16));
        if (GUILayout.Button("Selecionar...", GUILayout.Width(100)))
        {
            string selected = EditorUtility.OpenFolderPanel("Selecione a pasta de saída", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                // Converter caminho absoluto para relativo ao projeto (ex: Assets/...)
                if (selected.StartsWith(Application.dataPath))
                    folderPath = "Assets" + selected.Substring(Application.dataPath.Length);
                else
                    EditorUtility.DisplayDialog("Aviso", "Selecione uma pasta dentro de 'Assets'!", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Elementos do Enum:", EditorStyles.boldLabel);

        // Lista de elementos com rolagem
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        
        //GUILayout.Space(10);
        //GUILayout.Label("Elementos do Enum:", EditorStyles.boldLabel);

        //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        for (int i = 0; i < elements.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            elements[i] = EditorGUILayout.TextField(elements[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                elements.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("+ Adicionar Elemento"))
            elements.Add("NovoElemento");

        GUILayout.Space(15);

        if (GUILayout.Button("📜 Gerar Enum"))
        {
            GenerateEnumFile();
        }
    }

    private void GenerateEnumFile()
    {
        if (string.IsNullOrEmpty(enumName))
        {
            EditorUtility.DisplayDialog("Erro", "O nome do enum não pode estar vazio.", "OK");
            return;
        }

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"{enumName}.cs");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// Gerado automaticamente pelo EnumGeneratorWindow");
            writer.WriteLine($"public enum {enumName}");
            writer.WriteLine("{");
            foreach (string element in elements)
            {
                if (!string.IsNullOrWhiteSpace(element))
                {
                    string cleanName = MakeSafeIdentifier(element);
                    writer.WriteLine($"    {cleanName},");
                }
            }
            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Sucesso", $"Enum '{enumName}' gerado com sucesso em:\n{filePath}", "OK");
    }

    private string MakeSafeIdentifier(string name)
    {
        // Remove espaços e caracteres inválidos
        string safe = name.Trim().Replace(" ", "_").Replace("-", "_");
        // Garante que começa com letra
        if (char.IsDigit(safe[0]))
            safe = "_" + safe;
        return safe;
    }
}
