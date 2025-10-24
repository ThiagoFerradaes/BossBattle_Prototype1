#if UNITY_EDITOR
using System.Collections.Generic;

namespace NameSpaceEditor
{
    [System.Serializable]
    public class NamespacePreview
    {
        public string TypeName;      // Nome do tipo (ex: UsingInfo)
        public string Namespace;     // Namespace onde está definido
        public string TargetFile;    // Caminho do arquivo onde foi encontrado
        public bool CanBeModified;   // Se pode ser alterado
    }

    [System.Serializable]
    public class AssemblerUsings
    {
        public List<string> usings;
        public string targetFile;
    }

    [System.Serializable]
    public class AsmdefData
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public bool autoReferenced;
        public string[] precompiledReferences;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
        public bool noEngineReferences;
    }

    [System.Serializable]
    public class VersionDefine
    {
        public string name;
        public string expression;
        public string define;
    }
}
#endif