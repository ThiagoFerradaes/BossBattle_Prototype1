#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace NameSpaceEditor
{
    public static class NamespaceRegistry
    {
        private static readonly string RegistryPath = "Assets/Editor/NamespaceRegistry.json";
    
        /// <summary>
        /// Atualiza o arquivo JSON com todos os namespaces e tipos encontrados.
        /// </summary>
        public static Task UpdateRegistry(Dictionary<string, List<string>> data)
        {
            var mappings = data.Select(pair => new NamespaceMapping
            {
                Namespace = pair.Key,
                Types = pair.Value.Distinct().ToList()
            }).ToList();

            var json = JsonUtility.ToJson(new NamespaceWrapper { Mappings = mappings }, true);
            File.WriteAllText(RegistryPath, json);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Carrega o registro salvo no disco.
        /// </summary>
        public static Dictionary<string, List<string>> LoadRegistry()
        {
            if (!File.Exists(RegistryPath))
                return new Dictionary<string, List<string>>();

            var json = File.ReadAllText(RegistryPath);
            var wrapper = JsonUtility.FromJson<NamespaceWrapper>(json);
            return wrapper.Mappings.ToDictionary(m => m.Namespace, m => m.Types);
        }

        /// <summary>
        /// Encontra o namespace de um tipo específico.
        /// </summary>
        public static string GetNamespaceOfType(string typeName)
        {
            var dict = LoadRegistry();
            foreach (var pair in dict)
                if (pair.Value.Contains(typeName))
                    return pair.Key;
            return null;
        }
    }

    [System.Serializable]
    internal class NamespaceWrapper
    {
        public List<NamespaceMapping> Mappings = new();
    }

    [System.Serializable]
    public class NamespaceMapping
    {
        public string Namespace;
        public List<string> Types;
    }
}

#endif