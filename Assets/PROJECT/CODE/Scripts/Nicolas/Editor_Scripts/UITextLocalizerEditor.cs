#if UNITY_EDITOR
using UnityEditor;

namespace PROJECT.Scripts.Editor
{
    /// <summary>
    /// Custom editor for UITextLocalizer component that provides a user-friendly interface
    /// to manage text localization settings in the Unity Inspector.
    /// </summary>
    [CustomEditor(typeof(UITextLocalizer))]
    public class UITextLocalizerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Draws and handles the custom inspector GUI for the UITextLocalizer component.
        /// Provides options to switch between single and list modes for text localization.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serialized object representation
            serializedObject.Update();

            // Get and display the list mode toggle property
            SerializedProperty useListMode = serializedObject.FindProperty("useListMode");
            EditorGUILayout.PropertyField(useListMode);

            if (useListMode.boolValue)
            {
                // Display List Mode section
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("List Mode", EditorStyles.boldLabel);

                // Display the list of text boxes
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textBoxesList"), true);
            }
            else
            {
                // Display Single Mode section
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Single Mode", EditorStyles.boldLabel);

                // Display a single text box field
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textBox"));
            }

            // Apply any modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif