#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MyEnum;

namespace PROJECT.Scripts.Editor
{
    /// <summary>
    /// Custom editor for the TextBoxes component to manage localized text content
    /// </summary>
    [CustomEditor(typeof(TextBoxesSo))]
    public class TextBoxesEditor : UnityEditor.Editor
    {
        #region Variables
        private EnumLanguage _newLanguage;
        private string _newText = "";
        private bool _showAddSection;
        private Vector2 _scrollPos;

        private GUIStyle _headerStyle;
        private GUIStyle _textAreaStyle;
        private GUIStyle _boxStyle;
        #endregion

        #region GUI Styles
        /// <summary>
        /// Initializes custom GUI styles used in the editor
        /// </summary>
        private void InitStyles()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 12
                };

                _textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true,
                    fontSize = 11,
                    padding = new RectOffset(6, 6, 4, 4)
                };

                _boxStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 4, 4)
                };
            }
        }
        #endregion

        #region Inspector GUI
        /// <summary>
        /// Draws the custom inspector GUI for managing localized texts
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitStyles();

            var textBoxes = (TextBoxesSo)target;
            var dict = textBoxes.GetDictionary();

            EditorGUILayout.LabelField("🌐 Text Box Editor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Manage localized texts by language. Edit directly or add new ones below.",
                MessageType.Info);
            EditorGUILayout.Space(4);

            // ====== EXISTING TEXTS LIST ======
            if (dict.Count == 0)
            {
                EditorGUILayout.HelpBox("No texts added yet.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.LabelField("Existing Entries", _headerStyle);
                EditorGUILayout.Space(2);

                EditorGUILayout.BeginVertical(_boxStyle);
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(300));

                foreach (var lang in new System.Collections.Generic.List<EnumLanguage>(dict.Keys))
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"🌍 {lang}", EditorStyles.boldLabel, GUILayout.Width(120));

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("🗑️", GUILayout.Width(30)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Deletion",
                                $"Remove text for language '{lang}'?", "Yes", "No"))
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

                    // Editable text area
                    EditorGUI.BeginChangeCheck();
                    string newValue = EditorGUILayout.TextArea(dict[lang], _textAreaStyle, GUILayout.MinHeight(60));
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

            // ====== ADD SECTION ======
            _showAddSection = EditorGUILayout.Foldout(_showAddSection, "➕ Add or Replace Entry", true, _headerStyle);
            if (!_showAddSection) return;
            
            EditorGUILayout.BeginVertical(_boxStyle);

            EditorGUILayout.LabelField("Add / Replace Text", _headerStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Language", GUILayout.Width(70));
            _newLanguage = (EnumLanguage)EditorGUILayout.EnumPopup(_newLanguage);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Text");
            _newText = EditorGUILayout.TextArea(_newText, _textAreaStyle, GUILayout.MinHeight(80));

            EditorGUILayout.Space();

            if (GUILayout.Button("💾 Save Entry", GUILayout.Height(30)))
            {
                if (dict.ContainsKey(_newLanguage))
                {
                    if (!EditorUtility.DisplayDialog("Replace Existing?",
                            $"Text for '{_newLanguage}' already exists. Do you want to replace it?", "Yes", "No"))
                        return;
                }

                Undo.RecordObject(textBoxes, "Add or Replace Text Entry");
                dict[_newLanguage] = _newText;
                EditorUtility.SetDirty(textBoxes);
                _newText = "";
                _showAddSection = false;
            }

            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}
#endif