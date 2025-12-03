using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using MyEnum;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages the room furniture system, including furniture placement, unlocking, and UI display.
/// Handles furniture inventory, room size management, and localization for tavern rooms.
/// </summary>
public class RoomSystem : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("ID")]
    [SerializeField]
    [Tooltip("Room Identifier"), Range(0, byte.MaxValue)] 
    private byte id;

    [Header("Level for room")]
    [SerializeField]
    [Tooltip("Dictionary for level and Array of available furniture slots in the room")]
    private RoomLevelDictionary levelRoom;
    
    [Header("Furniture Configuration")]
    [SerializeField]
    [Tooltip("List of furniture currently placed in the room")]
    private List<Furniture> listOfFurniture = new List<Furniture>();

    [SerializeField]
    [Tooltip("Character associated with this room")]
    private CharactersSo character;

    //[SerializeField]
    //[Tooltip("Array of available furniture slots in the room")]
    //private SlotFurnitureRoom[] slotFurnitureRooms;

    [SerializeField]
    [Tooltip("Current number of active furniture slots")]
    private byte numberOfFurniture;

    [Header("UI References")]
    [SerializeField]
    [Tooltip("Text component displaying furniture name")]
    private TMP_Text nameFurniture;

    [SerializeField]
    [Tooltip("Text component displaying furniture description")]
    private TMP_Text descriptionFurniture;

    [FormerlySerializedAs("RoomAtributes")]
    [Space(10)]
    [Header("Attributes")]
    [SerializeField]
    [Tooltip("Attributes of the room affecting character preferences")]
    private FurnitureFeaturesSo roomAttributes;
    
    [Space(50)]
    [Header("Debug")]
    [SerializeField]
    [Tooltip("Debug array of furniture features for testing")]
    private FurnitureFeaturesSo[] furnitureFeaturesSos;

    [FormerlySerializedAs("_lockedFurnitureBySize")] [Space(5), SerializedDictionary("Type", "Value"), SerializeField]
    private SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, int> lockedFurnitureBySize = new SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, int>();

    [Space(10)]
    [SerializeField] 
    [Tooltip("Character happiness value tracking")]
    private CharacterValue characterHappiness;
    
    #endregion

    #region Private Fields

    /// <summary>Reference to the game configuration for language settings</summary>
    private ConfigurationSo _config;

    private byte _currentLevel;
    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes furniture dictionaries and populates debug furniture on awake
    /// </summary>
    private void Awake()
    {
        if (furnitureFeaturesSos.Length == 0) return;

        foreach (var furnitureFeature in furnitureFeaturesSos)
        {
            RoomCanvasStatic.Instance.AddUnlockedFurniture(furnitureFeature.Size, furnitureFeature);
        }

        foreach (var slot in Enum.GetValues(typeof(TypeOfEnvironmentCharacteristicEnum)))
        {
            lockedFurnitureBySize.Add((TypeOfEnvironmentCharacteristicEnum)slot, 0);
        }

    }

    /// <summary>
    /// Initializes the furniture list and activates slots based on room size
    /// </summary>
    private void OnEnable()
    {
        InitializeConfiguration();
        
        if (listOfFurniture.Count != 0) return;

        if (!levelRoom.inheritedData.TryGetValue(_currentLevel, out SlotFurnitureRoom[] slot))
        {
            Debug.LogError("No room found for level " + _currentLevel);
            return;
        }
        
        for (byte i = 0; i < slot.Length; i++)
        {
            slot[i].gameObject.SetActive(true);
            listOfFurniture.Add(new Furniture());
        }

        AddPreferenceToCharacter();
    }

    /// <summary>
    /// Cleans up event subscriptions when the component is disabled
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Saves character happiness when the object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        SaveHappinessCharacter();
    }

    #endregion

    #region Functionality

    /// <summary>
    /// Calculates and adds character preference based on room attributes
    /// </summary>
    private void AddPreferenceToCharacter()
    {
        if (character is null)
        {
            return;
        }

        List<FurnitureFeaturesSo> list = new List<FurnitureFeaturesSo> { roomAttributes };

        list.AddRange(furnitureFeaturesSos);

        float preference = character.CalculateAllPreferences(list);

        characterHappiness.character = character.Character;
        characterHappiness.value = preference;
    }

    #endregion

    #region Localization

    /// <summary>
    /// Loads configuration and subscribes to language change events
    /// </summary>
    private void InitializeConfiguration()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }

    /// <summary>
    /// Unsubscribes from language change events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    /// <summary>
    /// Updates UI text based on the selected language
    /// </summary>
    /// <param name="lang">Target language enum</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        // Language update logic to be implemented
    }

    #endregion

    #region Furniture Management

    /// <summary>
    /// Adds a new furniture piece to the first available empty slot
    /// </summary>
    /// <param name="newFurniture">The furniture features to add</param>
    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        foreach (var furniture in listOfFurniture)
        {
            if (furniture.furniture == null)
            {
                furniture.AddFurniture(newFurniture);
                float preference = characterHappiness.value;

                foreach (var characteristic in newFurniture.GetAllCharacteristics())
                {
                    lockedFurnitureBySize[characteristic.Key] += characteristic.Value.value;

                    if (character is null)
                    {
                        continue;
                    }
                    preference += character.CalculatePreference(characteristic.Key, lockedFurnitureBySize[characteristic.Key]);
                }

                if (character is null)
                {
                    break;
                }

                characterHappiness.character = character.Character;
                characterHappiness.value = preference;

                break;
            }
        }
    }

    /// <summary>
    /// Removes a specific furniture piece from the room
    /// </summary>
    /// <param name="furnitureToRemove">The furniture features to remove</param>
    public void RemoveFurniture(FurnitureFeaturesSo furnitureToRemove)
    {
        foreach (var furniture in listOfFurniture.Where(furniture => furniture.furniture == furnitureToRemove))
        {
            furniture.RemoveFurniture();
            float preference = characterHappiness.value;

            foreach (var characteristic in furnitureToRemove.GetAllCharacteristics())
            {
                int beginner = lockedFurnitureBySize[characteristic.Key];
                int newValue = beginner - characteristic.Value.value;
                lockedFurnitureBySize[characteristic.Key] = newValue;

                if (character is null)
                {
                    continue;
                }
                preference = character.CalculateRemove(characteristic.Key,beginner, newValue, preference);
            }

            if (character is null)
            {
                break;
            }

            characterHappiness.character = character.Character;
            characterHappiness.value = preference;

            break;
        }
    }

    /// <summary>
    /// Increases the room size by activating additional furniture slots
    /// </summary>
    /// <param name="addSizeRoom">Number of slots to add to the room</param>
    public void UpdateSizeRoom(byte addSizeRoom)
    {
        numberOfFurniture += addSizeRoom;

        if (!levelRoom.inheritedData.TryGetValue(_currentLevel, out SlotFurnitureRoom[] slot))
        {
            Debug.LogError("No room found for level " + _currentLevel);
            return;
        }
        
        for (byte i = 0; i < numberOfFurniture; i++)
        {
            slot[i].gameObject.SetActive(true);
        }
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the text component displaying the furniture name
    /// </summary>
    /// <returns>Reference to the name text component</returns>
    public TMP_Text GetNameFurniture() => nameFurniture;

    /// <summary>
    /// Gets the text component displaying furniture description
    /// </summary>
    /// <returns>Reference to the description text component</returns>
    public TMP_Text GetDescriptionFurniture() => descriptionFurniture;

    /// <summary>
    /// Gets the room identifier
    /// </summary>
    /// <returns>The room ID</returns>
    public byte ID() => id;
    
    #endregion

    #region Save And Load

    /// <summary>
    /// Gets the room's furniture data for saving
    /// </summary>
    /// <returns>Tuple containing furniture dictionary, room ID, and character happiness</returns>
    public (Dictionary<byte, Furniture> furnitureDictionary , byte id, CharacterValue characterHappiness, byte slotAmount) GetFurniture()
    {
        Dictionary<byte,Furniture> furniture = new Dictionary<byte, Furniture>();
        
        for (byte i = 0; i < listOfFurniture.Count; i++)
        {
            furniture.Add(i, listOfFurniture[i]);
        }
        
        return (furniture, id, characterHappiness, _currentLevel);
    }

    /// <summary>
    /// Saves the character's happiness value to PlayerPrefs
    /// </summary>
    private void SaveHappinessCharacter()
    {
        PlayerPrefs.SetFloat(""+characterHappiness.character,characterHappiness.value);
    }

    /// <summary>
    /// Loads furniture and character data from the saved state
    /// </summary>
    /// <param name="furniture">Dictionary of saved furniture data</param>
    /// <param name="characteristic">Saved character happiness data</param>
    /// <param name="slotAmount"></param>
    public void LoadFurniture(Dictionary<byte, Furniture> furniture, CharacterValue characteristic, byte slotAmount)
    {
        _currentLevel = slotAmount;
        if (!levelRoom.inheritedData.TryGetValue(_currentLevel, out SlotFurnitureRoom[] slot))
        {
            Debug.LogError("No room found for level " + _currentLevel);
            return;
        }
        
        for (byte i = 0; i < slotAmount; i++)
        {
            slot[i].gameObject.SetActive(true);
        }
        
        for (byte i = 0; i < slot.Length; i++)
        {
            listOfFurniture.Add(new Furniture());
            
            if(furniture.Count == 0) continue;
            if(!furniture.TryGetValue(i, out var value)) continue;
            
            listOfFurniture[i].AddFurniture(value.furniture);
            
            slot[i].LoadFurniture(value.furniture);
        }
        
        characterHappiness = characteristic;
    }
    
    #endregion
    
}

/// <summary>
/// Represents a single furniture slot in the room
/// Handles furniture assignment and triggers events for furniture changes
/// </summary>
[Serializable]
public class Furniture
{
    /// <summary>Size category of the furniture in this slot</summary>
    public SizeOfFurnitureEnum sizeOfFurniture;

    /// <summary>Current furniture features assigned to this slot</summary>
    public FurnitureFeaturesSo furniture;
    
    /// <summary>Event triggered when furniture is added to this slot</summary>
    public event Action<FurnitureFeaturesSo> OnFurnitureAdded;

    /// <summary>Event triggered when furniture is removed from this slot</summary>
    public event Action<FurnitureFeaturesSo> OnFurnitureRemoved;

    /// <summary>
    /// Assigns new furniture to this slot and triggers the added event
    /// </summary>
    /// <param name="newFurniture">The furniture features to assign</param>
    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        furniture = newFurniture;
        sizeOfFurniture = newFurniture.Size;
        OnFurnitureAdded?.Invoke(furniture);
    }

    /// <summary>
    /// Removes furniture from this slot and triggers the removed event
    /// </summary>
    public void RemoveFurniture()
    {
        OnFurnitureRemoved?.Invoke(furniture);
        furniture = null;
    }
}

/// <summary>
/// Stores character happiness data including character reference and happiness value
/// </summary>
[Serializable]
public class CharacterValue
{
    /// <summary>Reference to the character</summary>
    public Character character;
    
    /// <summary>Current happiness value</summary>
    public float value;

    /// <summary>
    /// Creates a new character value entry
    /// </summary>
    /// <param name="c">Character reference</param>
    /// <param name="v">Initial happiness value</param>
    public CharacterValue(Character c, float v)
    {
        character = c;
        value = v;
    }
}

[Serializable]
public class RoomLevelDictionary
{
    [SerializedDictionary("Level", "Slot")][HideInInspector]
    public SerializedDictionary<byte, SlotFurnitureRoom[]> inheritedData = new();


#if UNITY_EDITOR
    [SerializedDictionary("Level", "Inherited")]
    public SerializedDictionary<byte, SlotFurnitureRoom[]> levelRoom = new();
    
#endif
  
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SlotFurnitureRoom))]
public class SlotFurnitureRoomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        if (!string.IsNullOrEmpty(label.text))
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }
        
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Pega a referência do objeto (o property inteiro é a referência ao MonoBehaviour)
        UnityEngine.Object targetObject = property.objectReferenceValue;

        if (targetObject != null && targetObject is SlotFurnitureRoom)
        {
            float spacing = 5f;
            float halfWidth = (position.width - spacing) / 2f;
            
            Rect objectRect = new Rect(position.x, position.y, halfWidth, position.height);
            Rect enumRect = new Rect(position.x + halfWidth + spacing, position.y, halfWidth, position.height);
            
            EditorGUI.PropertyField(objectRect, property, GUIContent.none);
            
            SerializedObject serializedTarget = new SerializedObject(targetObject as UnityEngine.Object);
            SerializedProperty slotTypeProp = serializedTarget.FindProperty("slotType");

            if (slotTypeProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(enumRect, slotTypeProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedTarget.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.LabelField(enumRect, "slotType não encontrado");
            }
        }
        else
        {

            EditorGUI.PropertyField(position, property, GUIContent.none);
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(RoomLevelDictionary))]
public class RoomLevelDictionaryDrawer : PropertyDrawer
{
    private const float ButtonHeight = 20f;
    private const float ButtonSpacing = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, label, true);
        
        // Espaço para os botões (aplicar / limpar / esconder)
        height += (ButtonHeight + ButtonSpacing) * 2;

        // Último botão (toggle herdados)
        height += ButtonHeight + ButtonSpacing;

        return height;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Campo principal
        Rect mainRect = new Rect(position.x, position.y, position.width,
            EditorGUI.GetPropertyHeight(property, label, true));

        EditorGUI.PropertyField(mainRect, property, label, true);

        DrawButtons(position, property, mainRect);

        EditorGUI.EndProperty();
    }


    private void DrawButtons(Rect position, SerializedProperty property, Rect mainRect)
    {
        float y = mainRect.y + mainRect.height + ButtonSpacing;

        float buttonWidth = (position.width) - 4;

        Rect leftBtn = new Rect(position.x, y, buttonWidth, ButtonHeight);

        if (GUI.Button(leftBtn, "Aplicar Herança"))
            ApplyInheritance(property);
        
    }


    private void ApplyInheritance(SerializedProperty root)
    {
        root.serializedObject.ApplyModifiedProperties();

        var targetObj = root.serializedObject.targetObject;
        RoomLevelDictionary obj = fieldInfo.GetValue(targetObj) as RoomLevelDictionary;

        if (obj == null)
        {
            Debug.LogError("Falha ao acessar RoomLevelDictionary.");
            return;
        }

        var dict = obj.levelRoom;
        obj.inheritedData.Clear();

        if (dict == null || dict.Count == 0)
        {
            Debug.LogWarning("Dictionary está vazio.");
            return;
        }

        var sortedKeys = dict.Keys.OrderBy(k => k).ToList();
        var processed = new Dictionary<byte, List<SlotFurnitureRoom>>();

        foreach (byte key in sortedKeys)
        {
            var accumulated = new List<SlotFurnitureRoom>();

            foreach (SlotFurnitureRoom item in from k in sortedKeys.TakeWhile(k => k <= key) from item in dict[k] where item != null && !accumulated.Contains(item) select item)
            {
                accumulated.Add(item);
            }

            processed[key] = accumulated;
            obj.inheritedData[key] = accumulated.ToArray();
        }

        EditorUtility.SetDirty(targetObj);
        Debug.Log("Herança aplicada!");
    }
}

#endif
