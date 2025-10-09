using UnityEditor;
using UnityEngine;

namespace PROJECT.Scripts.Editor
{
    [CustomEditor(typeof(TextBoxes))]
    public class TextBoxesEditor : UnityEditor.Editor
    {
        private EnumLanguage newLanguage;
        private string newText = "";
        private bool showAddSection = false;
        private Vector2 scrollPos;

        private GUIStyle headerStyle;
        private GUIStyle textAreaStyle;
        private GUIStyle boxStyle;

        private void InitStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 12
                };

                textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true,
                    fontSize = 11,
                    padding = new RectOffset(6, 6, 4, 4)
                };

                boxStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 4, 4)
                };
            }
        }

        public override void OnInspectorGUI()
        {
            InitStyles();

            var textBoxes = (TextBoxes)target;
            var dict = textBoxes.GetDictionary();

            EditorGUILayout.LabelField("🌐 Text Box Editor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Gerencie textos localizados por idioma. Edite diretamente ou adicione novos abaixo.",
                MessageType.Info);
            EditorGUILayout.Space(4);

            // ====== LISTA DE TEXTOS EXISTENTES ======
            if (dict.Count == 0)
            {
                EditorGUILayout.HelpBox("Nenhum texto adicionado ainda.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.LabelField("Existing Entries", headerStyle);
                EditorGUILayout.Space(2);

                EditorGUILayout.BeginVertical(boxStyle);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

                foreach (var lang in new System.Collections.Generic.List<EnumLanguage>(dict.Keys))
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"🌍 {lang}", EditorStyles.boldLabel, GUILayout.Width(120));

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("🗑️", GUILayout.Width(30)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Deletion",
                                $"Remover texto da língua '{lang}'?", "Yes", "No"))
                        {
                            Undo.RecordObject(textBoxes, "Remove Text Entry");
                            dict.Remove(lang);
                            EditorUtility.SetDirty(textBoxes);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            break;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    // Área de texto editável
                    EditorGUI.BeginChangeCheck();
                    string newValue = EditorGUILayout.TextArea(dict[lang], textAreaStyle, GUILayout.MinHeight(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(textBoxes, "Edit Text Entry");
                        dict[lang] = newValue;
                        EditorUtility.SetDirty(textBoxes);
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // ====== SEÇÃO DE ADIÇÃO ======
            showAddSection = EditorGUILayout.Foldout(showAddSection, "➕ Add or Replace Entry", true, headerStyle);
            if (showAddSection)
            {
                EditorGUILayout.BeginVertical(boxStyle);

                EditorGUILayout.LabelField("Add / Replace Text", headerStyle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Language", GUILayout.Width(70));
                newLanguage = (EnumLanguage)EditorGUILayout.EnumPopup(newLanguage);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Text");
                newText = EditorGUILayout.TextArea(newText, textAreaStyle, GUILayout.MinHeight(80));

                EditorGUILayout.Space();

                if (GUILayout.Button("💾 Save Entry", GUILayout.Height(30)))
                {
                    if (dict.ContainsKey(newLanguage))
                    {
                        if (!EditorUtility.DisplayDialog("Replace Existing?",
                                $"O texto para '{newLanguage}' já existe. Deseja substituir?", "Yes", "No"))
                            return;
                    }

                    Undo.RecordObject(textBoxes, "Add or Replace Text Entry");
                    dict[newLanguage] = newText;
                    EditorUtility.SetDirty(textBoxes);
                    newText = "";
                    showAddSection = false;
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}

