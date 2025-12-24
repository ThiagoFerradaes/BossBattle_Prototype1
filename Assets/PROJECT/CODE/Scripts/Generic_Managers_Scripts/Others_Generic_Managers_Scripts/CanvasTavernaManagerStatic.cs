using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTavernaManagerStatic : MonoBehaviour
{
    public static CanvasTavernaManagerStatic Instance;
    public static event Action OnTavernaLoaded;
    
    private void Awake()
    {
        Instance ??= this;
        OnTavernaLoaded?.Invoke();
    }
    
    [SerializeField]
    private DialogueSystem dialogueSystem;
    
    [SerializeField]
    private MapManager mapManager;

    [SerializeField]
    private Button editorRoomButton;
    
    [SerializeField]
    private StoreSystem storeSystem;
    
    [SerializeField]
    private TrainingFieldSystem trainingSystem;
    
    [SerializedDictionary("Character", "Prefab")]
    [SerializeField] private SerializedDictionary<Character, GameObject> characterPrefabs;
    
    public GameObject GetCharacterPrefab(Character character) => characterPrefabs[character];
    
    public IReadOnlyDictionary<Character, GameObject> CharacterPrefabs => characterPrefabs;
    
    public MapManager MapManager => mapManager;

    public DialogueSystem DialogueSystem => dialogueSystem;
    
    public Button EditorRoomButton => editorRoomButton;
    
    public StoreSystem StoreSystem => storeSystem;
    
    public TrainingFieldSystem TrainingSystem => trainingSystem;
}
