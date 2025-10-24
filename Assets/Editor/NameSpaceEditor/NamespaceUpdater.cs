#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace NameSpaceEditor
{
    public static class NamespaceUtility
    {
        /// <summary>
        /// Gera namespace baseado no caminho de um arquivo .cs (ignorando "Assets/" e o nome do arquivo).
        /// Ex: Assets/Project/CODE/Scripts/Player/PlayerController.cs -> Project.CODE.Scripts.Player
        /// </summary>
        public static string GenerateNamespaceFromFile(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return string.Empty;

            string path = assetPath.Replace("\\", "/");

            if (path.StartsWith("Assets/"))
                path = path.Substring("Assets/".Length);

            // Remove arquivo
            int lastSlash = path.LastIndexOf('/');
            if (lastSlash >= 0)
                path = path.Substring(0, lastSlash);

            // substituir / por . e espaços por _
            path = path.Replace("/", ".").Replace(" ", "_");

            // remove eventual '.' no fim
            if (path.EndsWith("."))
                path = path.Substring(0, path.Length - 1);

            return path;
        }

        /// <summary>
        /// Gera namespace para uma pasta root selecionada (mantendo toda a pasta).
        /// Se você passar Assets/Project/CODE/Scripts retorna Project.CODE.Scripts
        /// </summary>
        public static string GenerateNamespaceFromFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return string.Empty;

            string path = folderPath.Replace("\\", "/");
            if (path.StartsWith("Assets/"))
                path = path.Substring("Assets/".Length);

            // remover trailing slash se existir
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);

            path = path.Replace("/", ".").Replace(" ", "_");

            if (path.EndsWith("."))
                path = path.Substring(0, path.Length - 1);

            return path;
        }
    }

    public static class NamespaceUpdaterProcessor
    {
        // regex para detectar declarações de classe/struct/interface/enum e nome
        private static readonly Regex ClassRegex = new(@"\b(class|struct|interface|enum)\s+([A-Za-z0-9_]+)",
            RegexOptions.Compiled);

        private static readonly Regex UsingLineRegex = new(@"^\s*using\s+([A-Za-z0-9_.]+)\s*;\s*$", RegexOptions.Compiled);
        private static readonly Regex WordRegex = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

        private static readonly Regex NamespaceBlockRegex =
            new(@"\bnamespace\b\s+([A-Za-z0-9_.]+)\s*\{", RegexOptions.Compiled);

        /// <summary>
        /// Escaneia todos os .cs dentro da pasta rootPath e retorna um dicionário de ScriptUsingInfo.
        /// Também constroi um mapa de classes (nome -> arquivo) para resolver tipos sem namespace.
        /// </summary>
        public static Dictionary<string, ScriptUsingInfo> ScanScripts(string rootPath)
        {
            var result = new Dictionary<string, ScriptUsingInfo>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                Debug.LogError($"[NamespaceUpdater] Caminho inválido: {rootPath}");
                return result;
            }

            var allCsFiles = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

            // 1) Build class/enum/type map: name -> list of files that declare it
            var classMap = BuildClassMap(allCsFiles);

            // 2) For each file, build ScriptUsingInfo
            foreach (var file in allCsFiles)
            {
                try
                {
                    var info = new ScriptUsingInfo
                    {
                        ScriptPath = file,
                        RelativePath = file.Replace("\\", "/").Replace("Assets/", "")
                    };

                    info.Namespace = NamespaceUtility.GenerateNamespaceFromFile(file);

                    // parse existing usings and convert into UsingInfo (resolving type-only usings)
                    info.Usings = ExtractAndResolveUsings(file, rootPath, classMap);

                    // infer implicit usings by analyzing code references (class names used in body)
                    AddImplicitUsingsFromReferences(file, info, rootPath, classMap);

                    // dedupe usings by Name, preserving CanBeModified = true if any says true
                    info.Usings = info.Usings
                        .GroupBy(u => u.Name, StringComparer.OrdinalIgnoreCase)
                        .Select(g => new UsingInfo(g.Key,
                            g.Select(x => x.TargetFile).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                            g.Any(x => x.CanBeModified)))
                        .ToList();

                    result[file] = info;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[NamespaceUpdater] Erro processando {file}: {ex.Message}");
                }
            }

            return result;
        }

        /// <summary>
        /// Monta um mapa de nomes de tipos (classe/struct/enum/interface) para os arquivos que os declaram.
        /// Se um mesmo nome aparecer em vários arquivos, escolhe o primeiro encontrado (poderíamos melhorar).
        /// </summary>
        private static Dictionary<string, string> BuildClassMap(string[] allCsFiles)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in allCsFiles)
            {
                try
                {
                    string text = File.ReadAllText(file);
                    foreach (Match m in ClassRegex.Matches(text))
                    {
                        string typeName = m.Groups[2].Value;
                        if (!map.ContainsKey(typeName))
                            map[typeName] = file;
                    }
                }
                catch
                {
                }
            }

            return map;
        }

        /// <summary>
        /// Lê os usings do arquivo e resolve "using MyEnum;" (sem ponto) mapeando para namespace do arquivo que declara MyEnum.
        /// Se o namespace do tipo estiver dentro da pasta rootPath, canModify fica true.
        /// </summary>
        private static List<UsingInfo> ExtractAndResolveUsings(string filePath, string rootPath,
            Dictionary<string, string> classMap)
        {
            var usings = new List<UsingInfo>();
            string[] lines = File.ReadAllLines(filePath);

            foreach (var raw in lines)
            {
                var line = raw;
                var match = UsingLineRegex.Match(line);
                if (!match.Success) continue;

                string usingBody = match.Groups[1].Value.Trim(); // ex: UnityEngine OR MyEnum OR Project.CODE.Scripts.Player

                if (usingBody.Contains(".")) // já é um namespace
                {
                    bool canModify = IsNamespaceInsideRoot(usingBody, rootPath);
                    usings.Add(new UsingInfo(usingBody, FindRepresentativeFileForNamespace(usingBody, rootPath),
                        canModify));
                }
                else
                {
                    // single token: pode ser um type name - tentar resolver por classMap
                    if (classMap.TryGetValue(usingBody, out var targetFile))
                    {
                        string targetNs = NamespaceUtility.GenerateNamespaceFromFile(targetFile);
                        bool canModify = IsPathInsideRoot(targetFile, rootPath);
                        usings.Add(new UsingInfo(targetNs, targetFile, canModify));
                    }
                    else
                    {
                        // não conseguiu resolver — manter como nome simples (pode gerar erro, mas preservamos)
                        usings.Add(new UsingInfo(usingBody, string.Empty, false));
                    }
                }
            }

            return usings;
        }

        /// <summary>
        /// Analisa o corpo do arquivo (palavras) e adiciona usings de tipos referenciados localmente (classMap),
        /// caso o tipo esteja declarado dentro da mesma árvore 'rootPath' e não seja do próprio arquivo.
        /// </summary>
        private static void AddImplicitUsingsFromReferences(string filePath, ScriptUsingInfo info, string rootPath,
            Dictionary<string, string> classMap)
        {
            string text = File.ReadAllText(filePath);

            // remove header usings and namespace blocks to get body with code references
            string body = RemoveExistingNamespaceBlock(text);

            // find all candidate words (could be types)
            var words = WordRegex.Matches(body)
                .Cast<Match>()
                .Select(m => m.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var w in words)
            {
                if (classMap.TryGetValue(w, out var declaringFile))
                {
                    // se o tipo está declarado no mesmo arquivo, pular
                    if (Path.GetFullPath(declaringFile)
                        .Equals(Path.GetFullPath(filePath), StringComparison.OrdinalIgnoreCase))
                        continue;

                    string targetNs = NamespaceUtility.GenerateNamespaceFromFile(declaringFile);

                    // se já temos esse using, garantir canModify true se o destino estiver na root
                    var existing = info.Usings.FirstOrDefault(u =>
                        string.Equals(u.Name, targetNs, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                    {
                        existing.CanBeModified = existing.CanBeModified || IsPathInsideRoot(declaringFile, rootPath);
                        if (string.IsNullOrEmpty(existing.TargetFile) && !string.IsNullOrEmpty(declaringFile))
                            existing.TargetFile = declaringFile;
                    }
                    else
                    {
                        // adiciona
                        var canModify = IsPathInsideRoot(declaringFile, rootPath);
                        info.Usings.Add(new UsingInfo(targetNs, declaringFile, canModify));
                    }
                }
            }
        }

        /// <summary>
        /// Remove bloco namespace { ... } existente e devolve apenas o corpo (mantendo using, atributos, etc removidos).
        /// Usado para evitar inserir namespace duplicado.
        /// </summary>
        private static string RemoveExistingNamespaceBlock(string fullText)
        {
            // Esta função procura o primeiro "namespace X {", e remove até o fechamento correspondente de chaves.
            // Se não encontrar, retorna o texto original.
            var m = NamespaceBlockRegex.Match(fullText);
            if (!m.Success) return fullText;

            int startIndex = m.Index;
            // encontrar a primeira chave '{' após o match
            int braceOpen = fullText.IndexOf('{', m.Index);
            if (braceOpen < 0) return fullText;

            // agora balancear chaves para encontrar o fechamento correspondente
            int depth = 0;
            int i = braceOpen;
            for (; i < fullText.Length; i++)
            {
                if (fullText[i] == '{') depth++;
                else if (fullText[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        // conteúdo entre braceOpen+1 e i-1 é o body; retornamos só esse body (mas sem a indentação)
                        string body = fullText.Substring(braceOpen + 1, i - braceOpen - 1);
                        return body;
                    }
                }
            }

            return fullText;
        }

        /// <summary>
        /// retorna true se a namespace (texto com pontos) está dentro do namespace derivado de rootPath
        /// </summary>
        private static bool IsNamespaceInsideRoot(string ns, string rootPath)
        {
            if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(rootPath)) return false;
            string rootNs = NamespaceUtility.GenerateNamespaceFromFolder(rootPath);
            return ns.StartsWith(rootNs, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// verifica se um arquivo está dentro da pasta rootPath
        /// </summary>
        private static bool IsPathInsideRoot(string filePath, string rootPath)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(rootPath)) return false;
            var fileFull = Path.GetFullPath(filePath).Replace('\\', '/').TrimEnd('/');
            var rootFull = Path.GetFullPath(rootPath).Replace('\\', '/').TrimEnd('/');
            return fileFull.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tenta encontrar um arquivo representativo que pertença à namespace ns dentro da árvore rootPath.
        /// (Procura por qualquer arquivo .cs que tenha namespace igual ao ns).
        /// </summary>
        private static string FindRepresentativeFileForNamespace(string ns, string rootPath)
        {
            try
            {
                var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    string text = File.ReadAllText(f);
                    var m = NamespaceBlockRegex.Match(text);
                    if (m.Success)
                    {
                        var declared = m.Groups[1].Value;
                        if (string.Equals(declared, ns, StringComparison.OrdinalIgnoreCase))
                            return f;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Aplica novos usings e namespaces (reescreve arquivos).
        /// Remove block namespace anterior e cria o novo corretamente, deduplicando usings.
        /// </summary>
        public static Task ApplyNamespaceAndUsings(Dictionary<string, ScriptUsingInfo> allScripts, string rootPath)
        {
            foreach (var kv in allScripts)
            {
                string path = kv.Key;
                var info = kv.Value;

                if (!File.Exists(path))
                    continue;

                string originalText = File.ReadAllText(path);

                // extração do body (sem namespace antigo)
                string body = RemoveExistingNamespaceBlock(originalText);

                // também remover "using" linhas do topo do body caso existam (pois vamos inserir de novo)
                var lines = body.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList();
                // remove leading using lines
                while (lines.Count > 0 && UsingLineRegex.IsMatch(lines[0]))
                    lines.RemoveAt(0);

                // monta lista final de usings: a partir de info.Usings, prefer nomes que sejam namespaces (com ponto).
                var finalUsings = info.Usings
                    .Select(u => u.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(n => n)
                    .ToList();

                var builder = new StringBuilder();

                // escreve usings
                foreach (var u in finalUsings)
                    builder.AppendLine($"using {u};");

                if (finalUsings.Count > 0)
                    builder.AppendLine();

                // escreve namespace
                if (!string.IsNullOrEmpty(info.Namespace))
                {
                    builder.AppendLine($"namespace {info.Namespace}");
                    builder.AppendLine("{");
                }

                // escreve body (com indentação)
                foreach (var ln in lines)
                {
                    // evita linhas vazias no começo
                    builder.AppendLine("    " + ln);
                }

                if (!string.IsNullOrEmpty(info.Namespace))
                {
                    builder.AppendLine("}");
                }

                // normalize line endings
                var finalText = builder.ToString().Replace("\r\n", "\n").Replace("\r", "\n");

                File.WriteAllText(path, finalText, Encoding.UTF8);
            }
            AssetDatabase.Refresh();
        
            return Task.CompletedTask;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static async Task<List<List<string>>> SmartCleanUsings(string rootPath, Dictionary<string, List<string>> mapUsing)
        {
            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                Debug.LogError($"[NamespaceUpdater] Caminho inválido: {rootPath}");
                return null;
            }

            var namespaceUsingsMap = new Dictionary<string, AssemblerUsings>();
        
            foreach (var file in Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories))
            {
                try
                {
                    string content = File.ReadAllText(file);
                    string originalContent = content;

                
                    // 1️⃣ — Detectar namespace atual do arquivo
                    string currentNamespace = "";
                    var nsMatch = Regex.Match(content, @"namespace\s+([A-Za-z0-9_.]+)");
                    if (nsMatch.Success)
                        currentNamespace = nsMatch.Groups[1].Value;   if (nsMatch.Success)
                        
                        // 2️⃣ — Remove usings antigos (mantém System, Unity, TMP e AYellowpaper)
                        content = Regex.Replace(
                            content,
                            @"^\s*using\s+(?!(System|Unity|TMP|AYellowpaper|NaughtyAttributes|DG))([A-Za-z0-9_.]+);\s*\r?\n",
                            "",
                            RegexOptions.Multiline
                        );
                
                    // 3️⃣ — Coletar todos os identificadores do código (nomes de tipos usados)
                    var identifiers = Regex.Matches(content, @"\b[A-Z][A-Za-z0-9_]*\b")
                        .Cast<Match>()
                        .Select(m => m.Value)
                        .Distinct()
                        .ToList();
                
               
                    var newUsings = new HashSet<string>();

                    // 4️⃣ — Detecta tipos do projeto (mapUsing)
                    foreach (var ns in mapUsing)
                    {
                        // Evita adicionar using igual ao próprio namespace
                        if (ns.Key == currentNamespace)
                            continue;

                        foreach (var type in ns.Value)
                        {
                            if (identifiers.Contains(type))
                            {
                                newUsings.Add($"using {ns.Key};");
                                break;
                            }
                        }
                    }
                
                    if (newUsings.Count == 0)
                        continue;
                
                    // Montar novo conteúdo com os usings ordenados no topo
                    string finalContent = string.Join("\n", newUsings.OrderBy(u => u)) + "\n\n" + content;

                    if (finalContent != originalContent)
                    {
                        File.WriteAllText(file, finalContent);
                    }
                
                    var finalUsingMatches = Regex.Matches(finalContent, @"^\s*using\s+([A-Za-z0-9_.]+);\s*$", RegexOptions.Multiline);
                
                    foreach (Match match in finalUsingMatches)
                    {
                        string usingName = match.Groups[1].Value.Trim();
                        if (string.IsNullOrEmpty(currentNamespace))
                        {
                            continue;
                        }

                        // ----- Aplique as regras de filtragem -----
                        // 1) Ignora System*
                        if (usingName.StartsWith("System", StringComparison.Ordinal))
                            continue;

                        // 2) Ignora DG*
                        if (usingName.StartsWith("DG", StringComparison.Ordinal))
                            continue;

                        if (usingName.StartsWith("NaughtyAttributes", StringComparison.Ordinal))
                        {
                            usingName = "NaughtyAttributes.Core";
                        }
                    
                        if (usingName.StartsWith("TMPro", StringComparison.Ordinal))
                        {
                            usingName = "Unity.TextMeshPro";
                        }

                        if (usingName.StartsWith("UnityEngine.InputSystem", StringComparison.Ordinal))
                        {
                            usingName = "Unity.InputSystem";
                        }
                    
                        // 3) UnityEngine* e UnityEditor* -> aceita só se contiver ".UI"
                        if (usingName.StartsWith("UnityEngine", StringComparison.Ordinal) ||
                            usingName.StartsWith("UnityEditor", StringComparison.Ordinal))
                        {
                            if (!usingName.Contains(".UI"))
                                continue; // rejeita UnityEngine (sem .UI) e UnityEditor (sem .UI)
                        }
                    
                        if (!namespaceUsingsMap.ContainsKey(currentNamespace))
                        {
                            namespaceUsingsMap.Add(currentNamespace , new AssemblerUsings
                            {
                                usings = new List<string>(),
                                targetFile = file
                            });
                        }

                        if (!namespaceUsingsMap[currentNamespace].usings.Contains(usingName))
                            namespaceUsingsMap[currentNamespace].usings.Add(usingName);
                    }
                
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[NamespaceCleaner] Erro ao limpar {file}: {ex.Message}");

                }
            }

            foreach (var (nameSpace, assemblerUsings) in namespaceUsingsMap)
            {
                var usings = assemblerUsings.usings;
                var targetFile = assemblerUsings.targetFile;
            
                // --- 🔧 Gera o caminho do arquivo .asmdef com base no nome do assembly ---
                // Pega apenas a pasta onde o .cs está
                string folder = Path.GetDirectoryName(targetFile);
                // Define o nome do arquivo .asmdef = nameSpace.asmdef
                string asmdefPath = Path.Combine(folder, $"{nameSpace}.asmdef");
                // -------------------------------------------------
            
                AsmdefData asmdefData;

                if (File.Exists(asmdefPath))
                {
                    // Lê e atualiza asmdef existente
                    string existingJson = File.ReadAllText(asmdefPath);
                    asmdefData = JsonUtility.FromJson<AsmdefData>(existingJson);

                    // Se algo estiver vazio ou null, inicializa
                    asmdefData.references ??= new string[0];
                    asmdefData.rootNamespace ??= nameSpace;

                    // Atualiza referências (mantém só as válidas)
                    var currentRefs = new HashSet<string>(asmdefData.references);
                    var desiredRefs = new HashSet<string>(usings);

                    // Adiciona novas
                    currentRefs.UnionWith(desiredRefs);
                    // Remove as que não estão mais em uso
                    currentRefs.IntersectWith(desiredRefs);

                    asmdefData.references = currentRefs.ToArray();
                
                }
                else
                {
                    // Cria novo asmdef
                    asmdefData = new AsmdefData
                    {
                        name = nameSpace,
                        rootNamespace = nameSpace,
                        references = usings.ToArray(),
                        includePlatforms = new string[0],
                        excludePlatforms = new string[0],
                        allowUnsafeCode = false,
                        autoReferenced = true,
                        overrideReferences = false,
                        precompiledReferences = new string[0],
                        defineConstraints = new string[0],
                        versionDefines = new VersionDefine[0],
                        noEngineReferences = false
                    };
                
                }

                // Serializa e grava
                string json = JsonUtility.ToJson(asmdefData, true);
                File.WriteAllText(asmdefPath, json);
            }

            // Pega todas as subpastas
            var allDirs = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

            foreach (var dir in allDirs.Append(rootPath)) // inclui a raiz também
            {
                var csFiles = Directory.GetFiles(dir, "*.cs", SearchOption.TopDirectoryOnly);
                if (csFiles.Length == 0)
                    continue; // sem scripts → pula

                // Procura um namespace válido em qualquer arquivo .cs
                string nameSpace = DetectNamespace(csFiles);
                if (string.IsNullOrEmpty(nameSpace))
                {
                    // Se não tiver namespace, gera a partir do caminho
                    string relative = dir.Replace("Assets", "")
                        .Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    relative = relative.Replace("\\", "_")
                        .Replace("/", "_")
                        .Replace(" ", "_");
                    nameSpace = string.IsNullOrEmpty(relative) ? "RootAssembly" : relative;
                }

                // Caminho do asmdef
                string asmdefPath = Path.Combine(dir, $"{nameSpace}.asmdef");

                // Se já existir, pula (ou atualiza, se quiser)
                if (File.Exists(asmdefPath))
                {
                    continue;
                }

                // Cria novo asmdef básico
                var asmdefData = new AsmdefData
                {
                    name = nameSpace,
                    rootNamespace = nameSpace,
                    references = new string[0],
                    includePlatforms = new string[0],
                    excludePlatforms = new string[0],
                    allowUnsafeCode = false,
                    autoReferenced = true,
                    overrideReferences = false,
                    precompiledReferences = new string[0],
                    defineConstraints = new string[0],
                    versionDefines = new VersionDefine[0],
                    noEngineReferences = false
                };

                string json = JsonUtility.ToJson(asmdefData, true);
                File.WriteAllText(asmdefPath, json);
            
            }
        
            AssetDatabase.Refresh();
        
            var cyclicDependencies = await DetectCyclicDependencies(namespaceUsingsMap);
         
            return cyclicDependencies;
        }

        private static Task<List<List<string>>> DetectCyclicDependencies(Dictionary<string, AssemblerUsings> namespaceUsingsMap)
        {
            // Dicionário: nome do asmdef -> lista de referencias
            Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

            foreach (var (nameSpace, assemblerUsings) in namespaceUsingsMap)
            {
                var references = assemblerUsings.usings;
                graph[nameSpace] = references.ToList();
            }
        
            HashSet<string> visited = new HashSet<string>();
            HashSet<string> stack = new HashSet<string>();
            List<List<string>> cycles = new List<List<string>>();

            bool DFS(string node, List<string> path)
            {
                if (stack.Contains(node))
                {
                    // Encontrou ciclo!
                    int index = path.IndexOf(node);
                    cycles.Add(path.GetRange(index, path.Count - index));
                    return true;
                }

                if (visited.Contains(node))
                    return false;

                visited.Add(node);
                stack.Add(node);
                path.Add(node);

                if (graph.TryGetValue(node, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        DFS(neighbor, new List<string>(path));
                    }
                }

                stack.Remove(node);
                return false;
            }

            // Executar DFS para todos os nodes
            foreach (var node in graph.Keys)
            {
                DFS(node, new List<string>());
            }
        
            return Task.FromResult(cycles);
        }

        public static async Task UpdateNamespaceRegistry(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                Debug.LogError("[NamespaceUpdater] Caminho inválido para UpdateNamespaceRegistry!");
                return;
            }

            var namespaceMap = new Dictionary<string, List<string>>();

            // Regex que aceita modificadores entre 'public' e o tipo (ex: public abstract partial class ...)
            var typeDeclRegex = new Regex(@"\bpublic\s+(?:(?:abstract|sealed|static|partial|unsafe|new)\s+)*\s*(class|struct|enum|interface)\s+([A-Za-z_][A-Za-z0-9_]*)",
                RegexOptions.Multiline);

            foreach (var file in Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories))
            {
                try
                {
                    string content = File.ReadAllText(file);

                    var nsMatch = Regex.Match(content, @"namespace\s+([A-Za-z0-9_.]+)");
                    if (!nsMatch.Success)
                        continue;

                    string ns = nsMatch.Groups[1].Value;

                    var matches = typeDeclRegex.Matches(content);
                    if (matches.Count == 0)
                        continue;

                    var types = matches
                        .Cast<Match>()
                        .Select(m => m.Groups[2].Value)
                        .Distinct()
                        .ToList();

                    if (types.Count == 0)
                        continue;

                    if (!namespaceMap.ContainsKey(ns))
                        namespaceMap[ns] = new List<string>();

                    // evita duplicatas na lista final
                    namespaceMap[ns].AddRange(types);
                    namespaceMap[ns] = namespaceMap[ns].Distinct().ToList();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[NamespaceUpdater] Erro ao processar {file}: {ex.Message}");
                }
            }

            await NamespaceRegistry.UpdateRegistry(namespaceMap);
        }
    
        private static Dictionary<string, string> _unityTypeMap;

        private static string DetectNamespace(string[] csFiles)
        {
            foreach (var file in csFiles)
            {
                var lines = File.ReadLines(file);
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("namespace "))
                    {
                        var parts = line.Split(new[] { ' ', '{', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                            return parts[1].Trim();
                    }
                }
            }
            return null;
        }
    }
}
#endif