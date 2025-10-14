#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PROJECT.Scripts.Editor
{
    /// <summary>
    /// Custom property drawer for ClampedVar<T> that provides automatic value clamping based on type constraints.
    /// Supports various numeric types including sbyte, byte, short, ushort, int, float, and double.
    /// </summary>
    [CustomPropertyDrawer(typeof(ClampedVar<>))]
    public class ClampedVarDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the GUI for the ClampedVar property with appropriate value constraints.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label to display for this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("value");
            EditorGUI.BeginProperty(position, label, property);

            // Get the generic type argument (T)
            var fieldType = fieldInfo.FieldType;
            var genericType = fieldType.IsGenericType ? fieldType.GetGenericArguments()[0] : null;

            // Default range values
            var (min, max, isIntType) = GetTypeConstraints(genericType);

            // Draw the appropriate slider based on type
            DrawSlider(position, label, valueProp, min, max, isIntType);

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Determines the minimum, maximum, and type classification for a given type.
        /// </summary>
        private (float min, float max, bool isIntType) GetTypeConstraints(System.Type type)
        {
            if (type == typeof(sbyte))
                return (sbyte.MinValue, sbyte.MaxValue, true);
            if (type == typeof(byte))
                return (byte.MinValue, byte.MaxValue, true);
            if (type == typeof(short))
                return (short.MinValue, short.MaxValue, true);
            if (type == typeof(ushort))
                return (ushort.MinValue, ushort.MaxValue, true);
            if (type == typeof(int))
                return (int.MinValue / 10f, int.MaxValue / 10f, true);
            if (type == typeof(uint))
                return (uint.MinValue / 10f, uint.MaxValue / 10f, true);
            if (type == typeof(long))
                return (long.MinValue / 10f, long.MaxValue / 10f, true);
            if (type == typeof(ulong))
                return (ulong.MinValue / 10f, ulong.MaxValue / 10f, true);
            if (type == typeof(float) || type == typeof(double))
                return (-1f, 1f, false);

            return (0f, 1f, false); // Default constraints
        }

        /// <summary>
        /// Draws the appropriate slider control based on the value type.
        /// </summary>
        private void DrawSlider(Rect position, GUIContent label, SerializedProperty valueProp, 
            float min, float max, bool isIntType)
        {
            if (isIntType)
            {
                int value = valueProp.intValue;
                value = Mathf.RoundToInt(EditorGUI.Slider(position, label, value, min, max));
                valueProp.intValue = Mathf.Clamp(value, (int)min, (int)max);
            }
            else if (valueProp.propertyType == SerializedPropertyType.Float)
            {
                float value = valueProp.floatValue;
                value = EditorGUI.Slider(position, label, value, min, max);
                valueProp.floatValue = Mathf.Clamp(value, min, max);
            }
            else
            {
                EditorGUI.PropertyField(position, valueProp, label);
            }
        }
    }
}
#endif