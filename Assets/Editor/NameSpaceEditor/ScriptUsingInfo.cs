#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace NameSpaceEditor
{
    [Serializable]
    public class ScriptUsingInfo
    {
        public string ScriptPath; // Caminho completo
        public string RelativePath; // Caminho relativo (sem Assets/)
        public string Namespace; // Namespace calculado (pela pasta)
        public List<UsingInfo> Usings = new(); // Lista de usings detalhada
    }

    [Serializable]
    public class UsingInfo
    {
        public string Name; // Ex: "UnityEngine" or "Project.CODE.Scripts.Player"
        public string TargetFile; // Arquivo que define este namespace/type (se houver)
        public bool CanBeModified; // Se este using pode ser alterado pelo sistema

        public UsingInfo()
        {
        }

        public UsingInfo(string name, string targetFile, bool canModify)
        {
            Name = name;
            TargetFile = targetFile;
            CanBeModified = canModify;
        }
    }
}
#endif