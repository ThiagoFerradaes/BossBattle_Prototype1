using System;
using UnityEngine;

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

    public MapManager MapManager => mapManager;


    public DialogueSystem DialogueSystem => dialogueSystem;
}
