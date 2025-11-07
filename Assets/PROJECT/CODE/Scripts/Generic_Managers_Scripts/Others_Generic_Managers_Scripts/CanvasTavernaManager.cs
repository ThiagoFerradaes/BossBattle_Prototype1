using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTavernaManager : MonoBehaviour
{
    public static CanvasTavernaManager Instance;
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
    
    public MapManager MapManager => mapManager;

    public DialogueSystem DialogueSystem => dialogueSystem;
    
    public Button EditorRoomButton => editorRoomButton;
}
